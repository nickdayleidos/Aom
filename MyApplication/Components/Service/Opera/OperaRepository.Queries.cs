using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Employee;
using System.Text.RegularExpressions;
using EmployeeEntity = MyApplication.Components.Model.AOM.Employee.Employees;

namespace MyApplication.Components.Service
{
    public sealed partial class OperaRepository
    {
        public async Task<(IReadOnlyList<OperaRequest> Items, int Total)> SearchAsync(OperaQuery q, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

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

            // Hierarchy Filters
            if (q.ManagerIds?.Count > 0)
                qry = qry.Where(x => x.Details.ManagerId.HasValue && q.ManagerIds.Contains(x.Details.ManagerId.Value));
            else if (q.ManagerId.HasValue)
                qry = qry.Where(x => x.Details.ManagerId == q.ManagerId);

            if (q.SupervisorIds?.Count > 0)
                qry = qry.Where(x => x.Details.SupervisorId.HasValue && q.SupervisorIds.Contains(x.Details.SupervisorId.Value));
            else if (q.SupervisorId.HasValue)
                qry = qry.Where(x => x.Details.SupervisorId == q.SupervisorId);

            // Type/SubType Filters
            if (q.ActivityTypeIds?.Count > 0)
                qry = qry.Where(x => q.ActivityTypeIds.Contains(x.Request.ActivityTypeId));
            else if (q.ActivityTypeId.HasValue)
                qry = qry.Where(x => x.Request.ActivityTypeId == q.ActivityTypeId);

            if (q.ActivitySubTypeIds?.Count > 0)
                qry = qry.Where(x => q.ActivitySubTypeIds.Contains(x.Request.ActivitySubTypeId));
            else if (q.ActivitySubTypeId.HasValue)
                qry = qry.Where(x => x.Request.ActivitySubTypeId == q.ActivitySubTypeId);

            // SubmittedBy Filter
            if (q.SubmittedBys?.Count > 0)
                qry = qry.Where(x => x.Request.SubmittedBy != null && q.SubmittedBys.Contains(x.Request.SubmittedBy));

            // Name/ID Search
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
                        (x.Request.Employees!.LastName + ", " + x.Request.Employees!.FirstName +
                            (x.Request.Employees!.MiddleInitial == null ? "" : " " + x.Request.Employees!.MiddleInitial)).Contains(t) ||
                        (x.Request.Employees!.FirstName + " " + x.Request.Employees!.LastName).Contains(t) ||
                        EF.Functions.Like(x.Request.Employees!.FirstName, $"%{t}%") ||
                        EF.Functions.Like(x.Request.Employees!.LastName, $"%{t}%")
                    );
                }
            }

            // Status + Date Filters
            if (q.StatusIds?.Count > 0)
                qry = qry.Where(x => x.Request.OperaStatusId.HasValue && q.StatusIds.Contains(x.Request.OperaStatusId.Value));
            else if (q.StatusId.HasValue)
                qry = qry.Where(x => x.Request.OperaStatusId == q.StatusId.Value);

            if (q.FromUtc.HasValue) qry = qry.Where(x => x.Request.StartTime >= q.FromUtc.Value);
            if (q.ToUtc.HasValue) qry = qry.Where(x => x.Request.StartTime < q.ToUtc.Value);

            var total = await qry.CountAsync(ct);
            var items = await qry
                .OrderByDescending(x => x.Request.SubmitTime)
                .Take(q.Take)
                .Select(x => x.Request)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<IReadOnlyList<OperaTimeframe>> GetTimeframesAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.OperaTimeframe.AsNoTracking().OrderBy(t => t.Id).ToListAsync(ct);
        }

        public async Task<IReadOnlyList<string>> GetOperaSubmittersAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.OperaRequests
                .AsNoTracking()
                .Where(x => x.SubmittedBy != null && x.SubmittedBy != "")
                .Select(x => x.SubmittedBy!)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(ct);
        }

        public async Task<DateTime?> GetStartOfShiftAsync(int employeeId, DateTime dateEt, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var history = await db.EmployeeHistory
                .AsNoTracking()
                .Where(h => h.EmployeeId == employeeId && h.EffectiveDate <= dateEt.Date && h.IsActive == true)
                .OrderByDescending(h => h.EffectiveDate)
                .ThenByDescending(h => h.Id)
                .Select(h => new { h.ScheduleRequestId })
                .FirstOrDefaultAsync(ct);

            if (history?.ScheduleRequestId == null) return null;

            var schedules = await db.AcrSchedules
                .AsNoTracking()
                .Where(s => s.AcrRequestId == history.ScheduleRequestId)
                .ToListAsync(ct);

            if (!schedules.Any()) return null;

            var dayStart = dateEt.Date;
            var dayEnd = dayStart.AddDays(1);
            var checkDates = new[]
            {
                DateOnly.FromDateTime(dayStart.AddDays(-1)),
                DateOnly.FromDateTime(dayStart)
            };

            foreach (var date in checkDates)
            {
                var dow = date.DayOfWeek;
                foreach (var sched in schedules)
                {
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
                        var shiftStart = date.ToDateTime(s.Value);
                        var shiftEnd = date.ToDateTime(e.Value);
                        if (shiftEnd < shiftStart) shiftEnd = shiftEnd.AddDays(1);
                        if (shiftStart < dayEnd && shiftEnd > dayStart) return shiftStart;
                    }
                }
            }

            return null;
        }

        public async Task<DateTime?> GetEndOfShiftAsync(int employeeId, DateTime startEt, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var history = await db.EmployeeHistory
                .AsNoTracking()
                .Where(h => h.EmployeeId == employeeId && h.EffectiveDate <= startEt.Date && (h.IsActive == true))
                .OrderByDescending(h => h.EffectiveDate)
                .ThenByDescending(h => h.Id)
                .Select(h => new { h.ScheduleRequestId })
                .FirstOrDefaultAsync(ct);

            if (history?.ScheduleRequestId == null) return null;

            var schedules = await db.AcrSchedules
                .AsNoTracking()
                .Where(s => s.AcrRequestId == history.ScheduleRequestId)
                .ToListAsync(ct);

            if (!schedules.Any()) return null;

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
                        var shiftStart = date.ToDateTime(s.Value);
                        var shiftEnd = date.ToDateTime(e.Value);
                        if (shiftEnd < shiftStart) shiftEnd = shiftEnd.AddDays(1);
                        if (shiftStart <= startEt && startEt < shiftEnd) return shiftEnd;
                    }
                }
            }

            return null;
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

        public async Task<OperaRequest> GetRequestByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return (await db.OperaRequests.FindAsync(new object[] { id }, ct))!;
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
                    q = q.Where(e =>
                        (e.LastName + ", " + e.FirstName + (e.MiddleInitial == null ? "" : " " + e.MiddleInitial)).Contains(term)
                        || (e.FirstName + " " + e.LastName).Contains(term)
                        || EF.Functions.Like(e.FirstName, $"%{term}%")
                        || EF.Functions.Like(e.LastName, $"%{term}%")
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

        public async Task<List<DetailedSchedule>> GetEmployeeWeeklyScheduleAsync(int employeeId, DateOnly weekStart, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var latestHistory = await db.EmployeeHistory
                .AsNoTracking()
                .Where(h => h.EmployeeId == employeeId)
                .OrderByDescending(h => h.EffectiveDate)
                .ThenByDescending(h => h.Id)
                .Select(h => h.ScheduleRequestId)
                .FirstOrDefaultAsync(ct);

            if (latestHistory == null) return new List<DetailedSchedule>();

            var acrSched = await db.AcrSchedules
                .AsNoTracking()
                .Where(s => s.AcrRequestId == latestHistory && s.ShiftNumber == 1)
                .FirstOrDefaultAsync(ct);

            if (acrSched == null) return new List<DetailedSchedule>();

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
                    if (e.Value < s.Value) endDt = endDt.AddDays(1);

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

        public async Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var data = await (
                from m in db.Managers.AsNoTracking()
                join e in db.Employees.AsNoTracking() on m.EmployeeId equals e.Id
                where m.IsActive == true && e.IsActive == true
                orderby e.LastName, e.FirstName
                select new { m.Id, e.LastName, e.FirstName, e.MiddleInitial }
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
                select new { s.Id, e.LastName, e.FirstName, e.MiddleInitial }
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
