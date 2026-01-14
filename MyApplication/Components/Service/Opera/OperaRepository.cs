using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Common.Time;
using System.Text.RegularExpressions;
using EmployeeEntity = MyApplication.Components.Model.AOM.Employee.Employees;

namespace MyApplication.Components.Service
{
    public sealed class OperaRepository : IOperaRepository
    {
        private readonly IDbContextFactory<AomDbContext> _factory;

        public OperaRepository(IDbContextFactory<AomDbContext> factory) => _factory = factory;

        public async Task<(IReadOnlyList<OperaRequest> Items, int Total)> SearchAsync(OperaQuery q, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            // 1. Join OperaRequests with EmployeeCurrentDetails
            var qry = db.OperaRequests
                .AsNoTracking()
                .Include(x => x.Employees)
                .Include(x => x.ActivityType)
                .Include(x => x.ActivitySubType)
                .Join(db.EmployeeCurrentDetails,
                      r => r.EmployeeId,
                      d => d.EmployeeId,
                      (r, d) => new { Request = r, Details = d })
                .AsQueryable();

            // 2. Apply Hierarchy Filters
            if (q.ManagerId.HasValue) qry = qry.Where(x => x.Details.ManagerId == q.ManagerId);
            if (q.SupervisorId.HasValue) qry = qry.Where(x => x.Details.SupervisorId == q.SupervisorId);

            // --- NEW: Apply Type/SubType Filters ---
            if (q.ActivityTypeId.HasValue) qry = qry.Where(x => x.Request.ActivityTypeId == q.ActivityTypeId);
            if (q.ActivitySubTypeId.HasValue) qry = qry.Where(x => x.Request.ActivitySubTypeId == q.ActivitySubTypeId);
            // ---------------------------------------

            // 3. Apply Name/ID Search
            if (!string.IsNullOrWhiteSpace(q.NameOrId))
            {
                var t = q.NameOrId.Trim();
                if (int.TryParse(t, out var asInt))
                {
                    qry = qry.Where(x => x.Request.EmployeeId == asInt || x.Request.RequestId == asInt);
                }
                else
                {
                    t = Regex.Replace(t, @"\s+", " ");
                    qry = qry.Where(x =>
                        (x.Request.Employees.LastName + ", " + x.Request.Employees.FirstName + (x.Request.Employees.MiddleInitial == null ? "" : " " + x.Request.Employees.MiddleInitial)).Contains(t) ||
                        (x.Request.Employees.FirstName + " " + x.Request.Employees.LastName).Contains(t) ||
                        EF.Functions.Like(x.Request.Employees.FirstName, $"%{t}%") ||
                        EF.Functions.Like(x.Request.Employees.LastName, $"%{t}%")
                    );
                }
            }


            // 4. Apply Status and Date Filters
            if (q.StatusId.HasValue) qry = qry.Where(x => x.Request.OperaStatusId == q.StatusId.Value);
            if (q.FromUtc.HasValue) qry = qry.Where(x => x.Request.StartTime >= q.FromUtc.Value);
            if (q.ToUtc.HasValue) qry = qry.Where(x => x.Request.StartTime < q.ToUtc.Value);

            // 5. Execute
            var total = await qry.CountAsync(ct);

            var items = await qry
                .OrderByDescending(x => x.Request.SubmitTime) // <--- UPDATED: Order by SubmitTime
                .Take(q.Take)
                .Select(x => x.Request)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<IReadOnlyList<OperaSubTypeDto>> GetSubTypesAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.ActivitySubTypes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new OperaSubTypeDto(x.Id, x.Name, x.ActivityTypeId))
                .ToListAsync(ct);
        }

        public async Task<OperaRequest?> GetAsync(int requestId, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.OperaRequests
                  .Include(x => x.Employees)
                  .Include(x => x.ActivityType)
                  .Include(x => x.ActivitySubType)
                  .FirstOrDefaultAsync(x => x.RequestId == requestId, ct);
        }

        public async Task<int> CreateManyAsync(IEnumerable<OperaRequest> requests, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            await db.OperaRequests.AddRangeAsync(requests, ct);
            return await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(OperaRequest req, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var existing = await db.OperaRequests.FirstOrDefaultAsync(x => x.RequestId == req.RequestId, ct);
            if (existing != null)
            {
                // Update editable fields
                existing.StartTime = req.StartTime;
                existing.EndTime = req.EndTime;
                existing.ActivityTypeId = req.ActivityTypeId;
                existing.ActivitySubTypeId = req.ActivitySubTypeId;
                existing.SubmitterComments = req.SubmitterComments;

                // --- NEW: Update Status and Reset Logic ---
                existing.OperaStatusId = req.OperaStatusId;

                // If resetting to Submitted (1) or Pending (6), clear approval/rejection history
                if (existing.OperaStatusId == 1 || existing.OperaStatusId == 6)
                {
                    existing.ApproveTime = null;
                    existing.ApproveBy = null;
                    existing.RejectedTime = null;
                    existing.RejectedBy = null;
                    existing.CancelledTime = null;
                    existing.CancelledBy = null;
                }
                // ------------------------------------------

                // Audit
                existing.LastUpdatedTime = Et.Now;
                existing.LastUpdatedBy = req.LastUpdatedBy;

                await db.SaveChangesAsync(ct);
            }
        }

        // File: MyApplication/Components/Service/Opera/OperaRepository.cs

        public async Task SetStatusAsync(int requestId, int statusId, string actor, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var r = await db.OperaRequests.FirstOrDefaultAsync(x => x.RequestId == requestId, ct);
            if (r == null) return;

            r.OperaStatusId = statusId;
            var nowEt = Et.Now;

            // --- NEW LOGIC START ---
            // Clear final state fields if returning to a non-final state (Submitted or Pending)
            if (statusId == 1 || statusId == 6)
            {
                r.ApproveTime = null;
                r.ApproveBy = null;
                r.RejectedTime = null;
                r.RejectedBy = null;
                r.CancelledTime = null;
                r.CancelledBy = null;
            }
            // --- NEW LOGIC END ---

            if (statusId == 2) // Approved
            {
                r.ApproveTime = nowEt;
                r.ApproveBy = actor;
            }
            else if (statusId == 5) // Rejected
            {
                r.RejectedTime = nowEt;
                r.RejectedBy = actor;
            }
            else if (statusId == 4) // Cancelled
            {
                r.CancelledTime = nowEt;
                r.CancelledBy = actor;
            }

            // Always update audit
            r.LastUpdatedTime = nowEt;
            r.LastUpdatedBy = actor;

            await db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<OperaStatus>> GetStatusesAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.OperaStatuses.AsNoTracking().OrderBy(x => x.Id).ToListAsync(ct);
        }

        public async Task<IReadOnlyList<EmployeeEntity>> SearchEmployeesAsync(string term, int take = 20, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            IQueryable<EmployeeEntity> q = db.Employees.AsNoTracking().Where(e => e.IsActive == true);

            if (!string.IsNullOrWhiteSpace(term))
            {
                if (int.TryParse(term, out var id))
                {
                    q = q.Where(e => e.Id == id);
                }
                else
                {
                    term = term.Trim();
                    // Robust Name Search
                    q = q.Where(e =>
                        (e.LastName + ", " + e.FirstName + (e.MiddleInitial == null ? "" : " " + e.MiddleInitial)).Contains(term)
                        ||
                        (e.FirstName + " " + e.LastName).Contains(term)
                        ||
                        EF.Functions.Like(e.FirstName, $"%{term}%")
                        ||
                        EF.Functions.Like(e.LastName, $"%{term}%")
                    );
                }
            }

            return await q.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).Take(take).ToListAsync(ct);
        }

        public async Task<Dictionary<int, string>> GetActivityTypesAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.ActivityTypes.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        }

        public async Task<Dictionary<int, string>> GetActivitySubTypesAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.ActivitySubTypes.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        }

        public async Task<Dictionary<int, string?>> GetEmployeeSiteTimeZonesAsync(IEnumerable<int> employeeIds, CancellationToken ct = default)
        {
            var distinctIds = employeeIds.Distinct().ToList();
            if (distinctIds.Count == 0) return new Dictionary<int, string?>();

            await using var db = await _factory.CreateDbContextAsync(ct);

            var histories = await db.EmployeeHistory
                .AsNoTracking()
                .Include(h => h.Site)
                .Where(h => distinctIds.Contains(h.EmployeeId))
                .Select(h => new { h.EmployeeId, h.EffectiveDate, h.IsActive, h.Site })
                .ToListAsync(ct);

            return histories
                .GroupBy(h => h.EmployeeId)
                .ToDictionary(g => g.Key, g =>
                {
                    var candidate = g.Where(h => h.IsActive == true)
                                     .OrderByDescending(h => h.EffectiveDate).FirstOrDefault()
                                     ?? g.OrderByDescending(h => h.EffectiveDate).FirstOrDefault();
                    return candidate?.Site?.TimeZoneWindows ?? candidate?.Site?.TimeZoneIana;
                });
        }

        // =========================================================================================
        // UPDATED: Get End Of Shift (Calculated from ACR Schedule, NOT DetailedSchedule)
        // =========================================================================================
        public async Task<DateTime?> GetEndOfShiftAsync(int employeeId, DateTime startEt, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            // 1. Find the history row effective for the given date (and Active)
            var history = await db.EmployeeHistory
                .AsNoTracking()
                .Where(h => h.EmployeeId == employeeId && h.EffectiveDate <= startEt.Date && h.IsActive == true)
                .OrderByDescending(h => h.EffectiveDate)
                .ThenByDescending(h => h.Id)
                .Select(h => new { h.ScheduleRequestId })
                .FirstOrDefaultAsync(ct);

            if (history?.ScheduleRequestId == null) return null;

            // 2. Fetch the schedule definition(s) associated with that ACR
            var schedules = await db.AcrSchedules
                .AsNoTracking()
                .Where(s => s.AcrRequestId == history.ScheduleRequestId)
                .ToListAsync(ct);

            if (!schedules.Any()) return null;

            // 3. Determine the shift logic
            // FIX: Convert DateTime to DateOnly explicitly so .ToDateTime() extension works
            var checkDates = new[]
            {
                DateOnly.FromDateTime(startEt.Date.AddDays(-1)),
                DateOnly.FromDateTime(startEt.Date)
            };

            foreach (var date in checkDates)
            {
                var dow = date.DayOfWeek;

                foreach (var sched in schedules)
                {
                    // Extract times for this DOW
                    TimeOnly? s = null, e = null;
                    switch (dow)
                    {
                        case DayOfWeek.Monday: s = sched.MondayStart; e = sched.MondayEnd; break;
                        case DayOfWeek.Tuesday: s = sched.TuesdayStart; e = sched.TuesdayEnd; break;
                        case DayOfWeek.Wednesday: s = sched.WednesdayStart; e = sched.WednesdayEnd; break;
                        case DayOfWeek.Thursday: s = sched.ThursdayStart; e = sched.ThursdayEnd; break;
                        case DayOfWeek.Friday: s = sched.FridayStart; e = sched.FridayEnd; break;
                        case DayOfWeek.Saturday: s = sched.SaturdayStart; e = sched.SaturdayEnd; break;
                        case DayOfWeek.Sunday: s = sched.SundayStart; e = sched.SundayEnd; break;
                    }

                    if (s.HasValue && e.HasValue)
                    {
                        // FIX: Now calling ToDateTime on a DateOnly object
                        var shiftStart = date.ToDateTime(s.Value);
                        var shiftEnd = date.ToDateTime(e.Value);

                        // Handle overnight wrap (e.g. 22:00 -> 06:00)
                        if (shiftEnd < shiftStart)
                        {
                            shiftEnd = shiftEnd.AddDays(1);
                        }

                        // Check if startEt is inside this shift window
                        // (Start <= Time < End)
                        if (shiftStart <= startEt && startEt < shiftEnd)
                        {
                            return shiftEnd;
                        }
                    }
                }
            }

            return null;
        }

        // =========================================================================================
        // UPDATED: Get Employee Weekly Schedule 
        // =========================================================================================
        public async Task<List<DetailedSchedule>> GetEmployeeWeeklyScheduleAsync(int employeeId, DateOnly weekStart, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            // 1. Find the ScheduleRequestId from the single most recent history row
            var latestHistory = await db.EmployeeHistory
                .AsNoTracking()
                .Where(h => h.EmployeeId == employeeId)
                .OrderByDescending(h => h.EffectiveDate)
                .ThenByDescending(h => h.Id)
                .Select(h => h.ScheduleRequestId)
                .FirstOrDefaultAsync(ct);

            if (latestHistory == null) return new List<DetailedSchedule>();

            // 2. Fetch the ACR Schedule Definition
            var acrSched = await db.AcrSchedules
                .AsNoTracking()
                .Where(s => s.AcrRequestId == latestHistory && s.ShiftNumber == 1) // Assuming Shift 1 for summary
                .FirstOrDefaultAsync(ct);

            if (acrSched == null) return new List<DetailedSchedule>();

            // 3. Generate 7 days for the requested week based on that static definition
            var result = new List<DetailedSchedule>();

            for (int i = 0; i < 7; i++)
            {
                DateOnly currentDate = weekStart.AddDays(i);
                var dayOfWeek = currentDate.DayOfWeek;

                TimeOnly? s = null, e = null;

                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday: s = acrSched.MondayStart; e = acrSched.MondayEnd; break;
                    case DayOfWeek.Tuesday: s = acrSched.TuesdayStart; e = acrSched.TuesdayEnd; break;
                    case DayOfWeek.Wednesday: s = acrSched.WednesdayStart; e = acrSched.WednesdayEnd; break;
                    case DayOfWeek.Thursday: s = acrSched.ThursdayStart; e = acrSched.ThursdayEnd; break;
                    case DayOfWeek.Friday: s = acrSched.FridayStart; e = acrSched.FridayEnd; break;
                    case DayOfWeek.Saturday: s = acrSched.SaturdayStart; e = acrSched.SaturdayEnd; break;
                    case DayOfWeek.Sunday: s = acrSched.SundayStart; e = acrSched.SundayEnd; break;
                }

                if (s.HasValue && e.HasValue)
                {
                    var startDt = currentDate.ToDateTime(s.Value);
                    var endDt = currentDate.ToDateTime(e.Value);

                    // Handle overnight shift wrapping
                    if (e.Value < s.Value)
                    {
                        endDt = endDt.AddDays(1);
                    }

                    result.Add(new DetailedSchedule
                    {
                        ScheduleDate = currentDate,
                        StartTime = startDt,
                        EndTime = endDt,
                        ActivityType = new ActivityType { Name = "Scheduled" },
                        ActivitySubType = new ActivitySubType { Name = "Shift" }
                    });
                }
            }

            return result;
        }

        // =========================================================================================
        // UPDATED: Hierarchy Lookups (Safe for Translation)
        // =========================================================================================
        public async Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var data = await (
                from m in db.Managers.AsNoTracking()
                join e in db.Employees.AsNoTracking() on m.EmployeeId equals e.Id
                where m.IsActive == true && e.IsActive == true
                orderby e.LastName, e.FirstName
                select new
                {
                    m.Id,
                    e.LastName,
                    e.FirstName,
                    e.MiddleInitial
                }
            ).ToListAsync(ct);

            return data
                .Select(x => new KeyValuePair<int, string>(
                    x.Id,
                    $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"
                ))
                .ToList();
        }

        public async Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var data = await (
                from s in db.Supervisors.AsNoTracking()
                join e in db.Employees.AsNoTracking() on s.EmployeeId equals e.Id
                where s.IsActive == true && e.IsActive == true
                orderby e.LastName, e.FirstName
                select new
                {
                    s.Id,
                    e.LastName,
                    e.FirstName,
                    e.MiddleInitial
                }
            ).ToListAsync(ct);

            return data
                .Select(x => new KeyValuePair<int, string>(
                    x.Id,
                    $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"
                ))
                .ToList();
        }
    }
}