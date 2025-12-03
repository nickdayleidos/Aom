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

            var qry = db.OperaRequests
                .AsNoTracking()
                .Include(x => x.Employees)
                .Include(x => x.ActivityType)
                .Include(x => x.ActivitySubType)
                .OrderByDescending(x => x.StartTime)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.NameOrId))
            {
                var t = q.NameOrId.Trim();
                if (int.TryParse(t, out var asInt))
                {
                    qry = qry.Where(x => x.EmployeeId == asInt || x.RequestId == asInt);
                }
                else
                {
                    t = Regex.Replace(t, @"\s+", " ");
                    qry = qry.Where(x =>
                        (x.Employees.FirstName != null && EF.Functions.Like(x.Employees.FirstName, $"%{t}%")) ||
                        (x.Employees.LastName != null && EF.Functions.Like(x.Employees.LastName, $"%{t}%")));
                }
            }

            if (q.StatusId.HasValue) qry = qry.Where(x => x.OperaStatusId == q.StatusId.Value);
            if (q.FromUtc.HasValue) qry = qry.Where(x => x.StartTime >= q.FromUtc.Value);
            if (q.ToUtc.HasValue) qry = qry.Where(x => x.StartTime < q.ToUtc.Value);

            var total = await qry.CountAsync(ct);
            var items = await qry.Take(q.Take).ToListAsync(ct);

            return (items, total);
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
                existing.StartTime = req.StartTime;
                existing.EndTime = req.EndTime;
                existing.ActivityTypeId = req.ActivityTypeId;
                existing.ActivitySubTypeId = req.ActivitySubTypeId;
                existing.SubmitterComments = req.SubmitterComments;

                // Audit
                existing.LastUpdatedTime = Et.Now;
                existing.LastUpdatedBy = req.LastUpdatedBy;

                await db.SaveChangesAsync(ct);
            }
        }

        public async Task SetStatusAsync(int requestId, int statusId, string actor, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var r = await db.OperaRequests.FirstOrDefaultAsync(x => x.RequestId == requestId, ct);
            if (r == null) return;

            r.OperaStatusId = statusId;
            var nowEt = Et.Now;

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
                    q = q.Where(e =>
                        (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{term}%")) ||
                        (e.LastName != null && EF.Functions.Like(e.LastName, $"%{term}%")));
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

        public async Task<DateTime?> GetEndOfShiftAsync(int employeeId, DateTime startEt, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var scheduleRow = await db.DetailedSchedule
                .Where(x => x.EmployeeId == employeeId && x.StartTime <= startEt && x.EndTime > startEt)
                .Select(x => new { x.ScheduleDate })
                .FirstOrDefaultAsync(ct);

            if (scheduleRow == null) return null;

            var eos = await db.DetailedSchedule
                .Where(x => x.EmployeeId == employeeId && x.ScheduleDate == scheduleRow.ScheduleDate)
                .MaxAsync(x => (DateTime?)x.EndTime, ct);

            return eos;
        }

        // =========================================================================================
        // UPDATED: Get Employee Weekly Schedule 
        // Logic: 1. Get MOST RECENT History row. 2. Use its ScheduleRequestId. 3. Build days.
        // =========================================================================================
        public async Task<List<DetailedSchedule>> GetEmployeeWeeklyScheduleAsync(int employeeId, DateOnly weekStart, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            // 1. Find the ScheduleRequestId from the single most recent history row
            var latestHistory = await db.EmployeeHistory
                .AsNoTracking()
                .Where(h => h.EmployeeId == employeeId)
                .OrderByDescending(h => h.EffectiveDate)
                .ThenByDescending(h => h.Id) // Tie-breaker for same effective date
                .Select(h => h.ScheduleRequestId)
                .FirstOrDefaultAsync(ct);

            if (latestHistory == null) return new List<DetailedSchedule>();

            // 2. Fetch the ACR Schedule Definition
            var acrSched = await db.AcrSchedules
                .AsNoTracking()
                .Where(s => s.AcrRequestId == latestHistory && s.ShiftNumber == 1) // Assuming Shift 1
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
    }
}