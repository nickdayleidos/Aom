using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Service.Employee.Dtos;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Common.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyApplication.Components.Service.Employee
{
    public sealed class EmployeesRepository
    {
        private readonly IDbContextFactory<AomDbContext> _factory;

        public EmployeesRepository(IDbContextFactory<AomDbContext> factory) => _factory = factory;

        public async Task<EmployeeFullDetailsVm> GetFullDetailsAsync(int employeeId)
        {
            using var ctx = await _factory.CreateDbContextAsync();

            var emp = await ctx.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == employeeId);
            if (emp == null) return null;

            var vm = new EmployeeFullDetailsVm
            {
                EmployeeId = emp.Id,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                MiddleInitial = emp.MiddleInitial,
                NmciEmail = emp.NmciEmail,
                UsnOperatorId = emp.UsnOperatorId,
                UsnAdminId = emp.UsnAdminId,
                CorporateEmail = emp.CorporateEmail,
                CorporateId = emp.CorporateId,
                DomainLoginName = emp.DomainLoginName,
                AwsId = emp.AwsId,
                AwsUsername = emp.Aws?.AwsUsername
            };

            var history = await ctx.EmployeeHistory.AsNoTracking()
                .Include(h => h.Employer)
                .Include(h => h.Site)
                .Include(h => h.Organization)
                .Include(h => h.SubOrganization)
                .Where(h => h.EmployeeId == employeeId)
                .OrderByDescending(h => h.EffectiveDate)
                .FirstOrDefaultAsync();

            if (history != null)
            {
                vm.HasHistory = true;
                vm.EffectiveDate = DateOnly.FromDateTime(history.EffectiveDate);
                vm.IsRemote = history.IsRemote ?? false;
                vm.IsLoa = history.IsLoa ?? false;
                vm.IsIntLoa = history.IsIntLoa ?? false;
                vm.Employer = history.Employer?.Name;
                vm.Site = history.Site?.SiteCode;
                vm.SiteTimeZoneId = history.Site?.TimeZoneWindows ?? history.Site?.TimeZoneIana;
                vm.Organization = history.Organization?.Name;
                vm.SubOrganization = history.SubOrganization?.Name;

                if (history.ManagerId.HasValue)
                {
                    vm.Manager = await ctx.Managers.Where(m => m.Id == history.ManagerId)
                        .Select(m => ctx.Employees.Where(e => e.Id == m.EmployeeId).Select(e => e.LastName + ", " + e.FirstName).FirstOrDefault())
                        .FirstOrDefaultAsync();
                }

                if (history.SupervisorId.HasValue)
                {
                    vm.Supervisor = await ctx.Supervisors.Where(s => s.Id == history.SupervisorId)
                        .Select(s => ctx.Employees.Where(e => e.Id == s.EmployeeId).Select(e => e.LastName + ", " + e.FirstName).FirstOrDefault())
                        .FirstOrDefaultAsync();
                }

                if (history.ScheduleRequestId.HasValue)
                {
                    var schedules = await ctx.AcrSchedules.AsNoTracking()
                        .Where(s => s.AcrRequestId == history.ScheduleRequestId.Value)
                        .ToListAsync();

                    string Fmt(TimeOnly? s, TimeOnly? e) => (s.HasValue && e.HasValue) ? $"{s:HH:mm}-{e:HH:mm}" : "OFF";

                    var s1 = schedules.FirstOrDefault(s => s.ShiftNumber == 1);
                    if (s1 != null)
                    {
                        vm.Schedule = new ScheduleDetailsVm
                        {
                            IsSplit = s1.IsSplitSchedule ?? false,
                            Mon = Fmt(s1.MondayStart, s1.MondayEnd),
                            Tue = Fmt(s1.TuesdayStart, s1.TuesdayEnd),
                            Wed = Fmt(s1.WednesdayStart, s1.WednesdayEnd),
                            Thu = Fmt(s1.ThursdayStart, s1.ThursdayEnd),
                            Fri = Fmt(s1.FridayStart, s1.FridayEnd),
                            Sat = Fmt(s1.SaturdayStart, s1.SaturdayEnd),
                            Sun = Fmt(s1.SundayStart, s1.SundayEnd),
                            MonStart = s1.MondayStart,
                            MonEnd = s1.MondayEnd,
                            TueStart = s1.TuesdayStart,
                            TueEnd = s1.TuesdayEnd,
                            WedStart = s1.WednesdayStart,
                            WedEnd = s1.WednesdayEnd,
                            ThuStart = s1.ThursdayStart,
                            ThuEnd = s1.ThursdayEnd,
                            FriStart = s1.FridayStart,
                            FriEnd = s1.FridayEnd,
                            SatStart = s1.SaturdayStart,
                            SatEnd = s1.SaturdayEnd,
                            SunStart = s1.SundayStart,
                            SunEnd = s1.SundayEnd
                        };

                        if (s1.IsSplitSchedule == true)
                        {
                            var s2 = schedules.FirstOrDefault(s => s.ShiftNumber == 2);
                            if (s2 != null)
                            {
                                vm.Schedule2 = new ScheduleDetailsVm
                                {
                                    IsSplit = true,
                                    Mon = Fmt(s2.MondayStart, s2.MondayEnd),
                                    Tue = Fmt(s2.TuesdayStart, s2.TuesdayEnd),
                                    Wed = Fmt(s2.WednesdayStart, s2.WednesdayEnd),
                                    Thu = Fmt(s2.ThursdayStart, s2.ThursdayEnd),
                                    Fri = Fmt(s2.FridayStart, s2.FridayEnd),
                                    Sat = Fmt(s2.SaturdayStart, s2.SaturdayEnd),
                                    Sun = Fmt(s2.SundayStart, s2.SundayEnd),
                                    MonStart = s2.MondayStart,
                                    MonEnd = s2.MondayEnd,
                                    TueStart = s2.TuesdayStart,
                                    TueEnd = s2.TuesdayEnd,
                                    WedStart = s2.WednesdayStart,
                                    WedEnd = s2.WednesdayEnd,
                                    ThuStart = s2.ThursdayStart,
                                    ThuEnd = s2.ThursdayEnd,
                                    FriStart = s2.FridayStart,
                                    FriEnd = s2.FridayEnd,
                                    SatStart = s2.SaturdayStart,
                                    SatEnd = s2.SaturdayEnd,
                                    SunStart = s2.SundayStart,
                                    SunEnd = s2.SundayEnd
                                };
                            }
                        }
                    }
                }

                if (history.OvertimeRequestId.HasValue)
                {
                    var ot = await ctx.AcrOvertimeSchedules.AsNoTracking().FirstOrDefaultAsync(o => o.AcrRequestId == history.OvertimeRequestId.Value);
                    if (ot != null)
                    {
                        var types = await ctx.AcrOvertimeTypes.AsNoTracking().ToDictionaryAsync(k => k.Id, v => v.Name);
                        string GetOt(int? id) => id.HasValue && types.ContainsKey(id.Value) ? types[id.Value] : "-";

                        vm.Overtime = new OvertimeDetailsVm
                        {
                            Mon = GetOt(ot.MondayTypeId),
                            Tue = GetOt(ot.TuesdayTypeId),
                            Wed = GetOt(ot.WednesdayTypeId),
                            Thu = GetOt(ot.ThursdayTypeId),
                            Fri = GetOt(ot.FridayTypeId),
                            Sat = GetOt(ot.SaturdayTypeId),
                            Sun = GetOt(ot.SundayTypeId)
                        };
                    }
                }
            }

            var breaks = await ctx.BreakSchedules.AsNoTracking().FirstOrDefaultAsync(b => b.EmployeeId == employeeId);
            if (breaks != null)
            {
                var templates = await ctx.BreakTemplates.AsNoTracking().ToDictionaryAsync(k => k.Id, v => v.Name);
                string GetBrk(int? id) => id.HasValue && templates.ContainsKey(id.Value) ? templates[id.Value] : "-";
                vm.StaticBreaks = new StaticBreakDetailsVm
                {
                    Mon = GetBrk(breaks.MondayTemplateId),
                    Tue = GetBrk(breaks.TuesdayTemplateId),
                    Wed = GetBrk(breaks.WednesdayTemplateId),
                    Thu = GetBrk(breaks.ThursdayTemplateId),
                    Fri = GetBrk(breaks.FridayTemplateId),
                    Sat = GetBrk(breaks.SaturdayTemplateId),
                    Sun = GetBrk(breaks.SundayTemplateID)
                };
            }

            vm.Skills = await ctx.Skills.AsNoTracking()
                 .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                 .Include(s => s.SkillType)
                 .Select(s => s.SkillType.Name)
                 .OrderBy(n => n)
                 .ToListAsync();

            vm.AcrHistory = await ctx.AcrRequests.AsNoTracking()
                .Where(r => r.EmployeeId == employeeId)
                .Include(r => r.AcrType)
                .Include(r => r.AcrStatus)
                .OrderByDescending(r => r.EffectiveDate)
                .Take(20)
                .Select(r => new AcrHistoryDto
                {
                    Id = r.Id,
                    Type = r.AcrType.Name,
                    Status = r.AcrStatus.Name,
                    EffectiveDate = r.EffectiveDate,
                    Submitted = r.SubmitTime
                }).ToListAsync();

            vm.OperaHistory = await ctx.OperaRequests.AsNoTracking()
                .Where(r => r.EmployeeId == employeeId)
                .Include(r => r.ActivityType)
                .Include(r => r.ActivitySubType)
                .Include(r => r.OperaStatus)
                .OrderByDescending(r => r.StartTime)
                .Take(20)
                .Select(r => new OperaHistoryDto
                {
                    RequestId = r.RequestId,
                    Type = r.ActivityType.Name,
                    SubType = r.ActivitySubType.Name,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    Status = r.OperaStatus.Name
                }).ToListAsync();

            return vm;
        }

        // =========================================================
        // Updated SearchAsync: Accepts TimeDisplayMode parameter
        // =========================================================
        public async Task<List<EmployeeListItem>> SearchAsync(string? query, bool activeOnly, TimeDisplayMode mode = TimeDisplayMode.Eastern, int take = 200, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var baseQuery = db.EmployeeCurrentDetails.AsQueryable();

            if (activeOnly) baseQuery = baseQuery.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim();
                var isId = int.TryParse(q, out var idValue);
                baseQuery = baseQuery.Where(x =>
                       (x.FirstName != null && EF.Functions.Like(x.FirstName, $"%{q}%"))
                    || (x.LastName != null && EF.Functions.Like(x.LastName, $"%{q}%"))
                    || (isId && x.EmployeeId == idValue)
                );
            }

            var items = await baseQuery
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .Take(take)
                .Select(x => new EmployeeListItem
                {
                    Id = x.EmployeeId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    IsActive = x.IsActive,
                    Manager = x.ManagerName,
                    Supervisor = x.SupervisorName,
                    Organization = x.OrganizationName,
                    SubOrganization = x.SubOrganizationName,
                })
                .ToListAsync(ct);

            if (!items.Any()) return items;

            // Fetch Details
            var empIds = items.Select(x => x.Id).ToList();

            var profiles = await db.EmployeeRoutingProfiles.AsNoTracking()
                .Where(p => empIds.Contains(p.EmployeeId))
                .Include(p => p.WeekdayProfile).Include(p => p.WeekendProfile)
                .ToListAsync(ct);

            var skills = await db.Skills.AsNoTracking()
                .Where(s => empIds.Contains(s.EmployeeId) && s.IsActive == true)
                .Include(s => s.SkillType)
                .ToListAsync(ct);

            var histories = await db.EmployeeHistory.AsNoTracking()
                .Where(h => empIds.Contains(h.EmployeeId) && h.ScheduleRequestId != null)
                .Select(h => new { h.EmployeeId, h.ScheduleRequestId, h.EffectiveDate })
                .ToListAsync(ct);

            var latestScheduleMap = histories
                .GroupBy(h => h.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.EffectiveDate).First())
                .ToDictionary(k => k.EmployeeId, v => v.ScheduleRequestId);

            var reqIds = latestScheduleMap.Values.Where(v => v.HasValue).Select(v => v.Value).Distinct().ToList();
            var schedules = new List<AcrSchedule>();
            if (reqIds.Any())
            {
                schedules = await db.AcrSchedules.AsNoTracking()
                    .Where(s => reqIds.Contains(s.AcrRequestId) && s.ShiftNumber == 1)
                    .ToListAsync(ct);
            }

            foreach (var item in items)
            {
                var p = profiles.FirstOrDefault(x => x.EmployeeId == item.Id);
                if (p != null)
                {
                    item.WeekdayProfile = p.WeekdayProfile?.Name;
                    item.WeekendProfile = p.WeekendProfile?.Name;
                }

                item.Skills = skills
                    .Where(s => s.EmployeeId == item.Id)
                    .Select(s => s.SkillType?.Name ?? "")
                    .OrderBy(n => n)
                    .ToList();

                if (latestScheduleMap.TryGetValue(item.Id, out var reqId) && reqId.HasValue)
                {
                    var sched = schedules.FirstOrDefault(s => s.AcrRequestId == reqId.Value);
                    // Pass 'mode' to helpers
                    item.Schedule = FormatSchedule(sched, mode);
                    item.DailySchedules = BuildDailySchedules(sched, mode);
                }
            }

            return items;
        }

        public async Task UpdateProfileAsync(EmployeeProfileUpdateDto dto)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var emp = await ctx.Employees.FindAsync(dto.Id);
            if (emp != null)
            {
                emp.FirstName = dto.FirstName; emp.LastName = dto.LastName; emp.MiddleInitial = dto.MiddleInitial;
                emp.NmciEmail = dto.NmciEmail; emp.UsnOperatorId = dto.UsnOperatorId; emp.UsnAdminId = dto.UsnAdminId;
                emp.CorporateEmail = dto.CorporateEmail; emp.CorporateId = dto.CorporateId;
                emp.DomainLoginName = dto.DomainLoginName; emp.AwsId = dto.AwsId;
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<AwsIdentifierLookupDto>> SearchAwsIdentifiersAsync(string query, CancellationToken ct = default)
        {
            using var ctx = await _factory.CreateDbContextAsync(ct);
            return await ctx.Identifiers.AsNoTracking()
                .Where(x => EF.Functions.Like(x.AwsUsername, $"%{query}%"))
                .Take(20)
                .Select(x => new AwsIdentifierLookupDto { Id = x.Id, AwsUsername = x.AwsUsername })
                .ToListAsync(ct);
        }

        public async Task<AwsIdentifierLookupDto?> GetAwsIdentifierAsync(int id)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Identifiers.Where(x => x.Id == id)
               .Select(x => new AwsIdentifierLookupDto { Id = x.Id, AwsUsername = x.AwsUsername })
               .FirstOrDefaultAsync();
        }

        // ====================================================================
        // NEW: GetDailySchedulesAsync with DetailedSchedule + AWS Status Logic
        // ====================================================================
        public async Task<ScheduleDashboardVm> GetDailySchedulesAsync(DateOnly date, TimeDisplayMode mode, CancellationToken ct = default)
        {
            if (date == DateOnly.MinValue) return new ScheduleDashboardVm { Date = date };

            using var ctx = await _factory.CreateDbContextAsync();

            // 1. Get raw schedule data
            var rawData = await ctx.DetailedSchedule.AsNoTracking()
                .Where(x => x.ScheduleDate == date)
                .Include(x => x.ActivityType)
                .Include(x => x.ActivitySubType)
                .Include(x => x.AwsStatus)
                .ToListAsync(ct);

            if (!rawData.Any()) return new ScheduleDashboardVm { Date = date };

            // 2. Get Employee/Site context (names, orgs, timezones)
            var empIds = rawData.Select(x => x.EmployeeId).Distinct().ToList();
            var histories = await ctx.EmployeeHistory.AsNoTracking()
                .Where(h => empIds.Contains(h.EmployeeId))
                // Optimistic: Get active history, or latest if none active
                .Include(h => h.Site)
                .Include(h => h.Organization)
                .Include(h => h.SubOrganization)
                .Include(h => h.Supervisor).ThenInclude(s => s.Employee)
                .Include(h => h.Employee) // For Employee Name
                .ToListAsync(ct);

            // Filter to best history row per employee
            var bestHistory = histories
                .GroupBy(h => h.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.IsActive == true).ThenByDescending(x => x.EffectiveDate).First())
                .ToList();

            var dashboard = new ScheduleDashboardVm { Date = date };

            // 3. Group by Site
            var groupedBySite = bestHistory.GroupBy(x => x.Site?.SiteCode ?? "Unknown")
                                           .OrderBy(g => g.Key);

            foreach (var siteGroup in groupedBySite)
            {
                var siteName = siteGroup.Key;
                // Grab TZ from first employee in site group
                var first = siteGroup.First();
                var siteTz = first.Site?.TimeZoneWindows ?? first.Site?.TimeZoneIana ?? "Eastern Standard Time";

                var siteVm = new SiteScheduleGroupVm { SiteName = siteName, TimeZoneId = siteTz };

                foreach (var hist in siteGroup)
                {
                    var empSchedules = rawData.Where(x => x.EmployeeId == hist.EmployeeId).OrderBy(x => x.StartTime).ToList();

                    var empVm = new EmployeeDailyScheduleVm
                    {
                        EmployeeId = hist.EmployeeId,
                        EmployeeName = hist.Employee != null ? $"{hist.Employee.LastName}, {hist.Employee.FirstName}" : $"#{hist.EmployeeId}",
                        OrganizationName = hist.Organization?.Name ?? "",
                        SubOrganizationName = hist.SubOrganization?.Name ?? "",
                        SupervisorName = hist.Supervisor?.Employee != null ? $"{hist.Supervisor.Employee.LastName}, {hist.Supervisor.Employee.FirstName}" : ""
                    };

                    string targetTzId = mode switch
                    {
                        TimeDisplayMode.Eastern => "Eastern Standard Time",
                        TimeDisplayMode.Mountain => "Mountain Standard Time",
                        _ => siteTz
                    };

                    var segments = new List<ScheduleSegmentVm>();
                    DateTime? minDt = null;
                    DateTime? maxDt = null;

                    foreach (var item in empSchedules)
                    {
                        var localStart = Tz.FromEtToSite(item.StartTime, targetTzId);
                        var localEnd = Tz.FromEtToSite(item.EndTime, targetTzId);

                        if (minDt == null || localStart < minDt) minDt = localStart;
                        if (maxDt == null || localEnd > maxDt) maxDt = localEnd;

                        segments.Add(new ScheduleSegmentVm
                        {
                            ActivityTypeId = item.ActivityTypeId,
                            ActivitySubTypeId = item.ActivitySubTypeId,
                            ActivityName = item.ActivityType?.Name ?? "?",
                            SubActivityName = item.ActivitySubType?.Name ?? "-",
                            AwsStatusId = item.AwsStatusId,
                            AwsStatusName = item.AwsStatus?.Name ?? "-",
                            Start = TimeOnly.FromDateTime(localStart),
                            End = TimeOnly.FromDateTime(localEnd),
                            IsImpacting = item.IsImpacting ?? false
                        });
                    }

                    empVm.Segments = segments;
                    empVm.ShiftString = (minDt.HasValue && maxDt.HasValue) ? $"{minDt.Value:HH:mm} - {maxDt.Value:HH:mm}" : "OFF";

                    if (segments.Any())
                        siteVm.Employees.Add(empVm);
                }

                siteVm.Employees = siteVm.Employees.OrderBy(e => e.EmployeeName).ToList();
                if (siteVm.Employees.Any())
                    dashboard.Sites.Add(siteVm);
            }

            return dashboard;
        }

        public async Task<EmployeeDetailDto?> GetDetailAsync(int employeeId, CancellationToken ct = default)
        {
            return null;
        }

        // =========================
        // HELPERS (With Mode)
        // =========================
        private List<DayScheduleDto> BuildDailySchedules(AcrSchedule? s, TimeDisplayMode mode)
        {
            var list = new List<DayScheduleDto>();
            if (s == null) return list;

            void Add(string d, TimeOnly? start, TimeOnly? end) =>
                list.Add(new DayScheduleDto { Day = d, Start = start, End = end });

            Add("Mon", Convert(s.MondayStart, mode), Convert(s.MondayEnd, mode));
            Add("Tue", Convert(s.TuesdayStart, mode), Convert(s.TuesdayEnd, mode));
            Add("Wed", Convert(s.WednesdayStart, mode), Convert(s.WednesdayEnd, mode));
            Add("Thu", Convert(s.ThursdayStart, mode), Convert(s.ThursdayEnd, mode));
            Add("Fri", Convert(s.FridayStart, mode), Convert(s.FridayEnd, mode));
            Add("Sat", Convert(s.SaturdayStart, mode), Convert(s.SaturdayEnd, mode));
            Add("Sun", Convert(s.SundayStart, mode), Convert(s.SundayEnd, mode));

            return list;
        }

        private string FormatSchedule(AcrSchedule? s, TimeDisplayMode mode)
        {
            if (s == null) return "-";

            var days = new[] {
                (d: "Mon", s: Convert(s.MondayStart, mode), e: Convert(s.MondayEnd, mode)),
                (d: "Tue", s: Convert(s.TuesdayStart, mode), e: Convert(s.TuesdayEnd, mode)),
                (d: "Wed", s: Convert(s.WednesdayStart, mode), e: Convert(s.WednesdayEnd, mode)),
                (d: "Thu", s: Convert(s.ThursdayStart, mode), e: Convert(s.ThursdayEnd, mode)),
                (d: "Fri", s: Convert(s.FridayStart, mode), e: Convert(s.FridayEnd, mode)),
                (d: "Sat", s: Convert(s.SaturdayStart, mode), e: Convert(s.SaturdayEnd, mode)),
                (d: "Sun", s: Convert(s.SundayStart, mode), e: Convert(s.SundayEnd, mode))
            };

            var active = days.Where(x => x.s.HasValue && x.e.HasValue).ToList();
            if (!active.Any()) return "OFF";

            var first = active.First();
            bool sameTime = active.All(x => x.s == first.s && x.e == first.e);

            if (sameTime)
            {
                string range = (active.Count == 5 && active[0].d == "Mon" && active[4].d == "Fri") ? "Mon-Fri" : $"{active.Count} Days";
                return $"{range} {first.s:HH:mm}-{first.e:HH:mm}";
            }
            return "Varies";
        }

        private TimeOnly? Convert(TimeOnly? time, TimeDisplayMode mode)
        {
            if (!time.HasValue) return null;
            if (mode == TimeDisplayMode.Eastern) return time;

            try
            {
                var et = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var targetZoneId = mode == TimeDisplayMode.Mountain ? "Mountain Standard Time" : "Eastern Standard Time";
                var target = TimeZoneInfo.FindSystemTimeZoneById(targetZoneId);
                var today = DateTime.Today;
                var dtEt = new DateTime(today.Year, today.Month, today.Day, time.Value.Hour, time.Value.Minute, 0);
                var dtTarget = TimeZoneInfo.ConvertTime(dtEt, et, target);
                return TimeOnly.FromDateTime(dtTarget);
            }
            catch { return time; }
        }
    }
}