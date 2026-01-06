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

            var emp = await ctx.Employees.AsNoTracking()
                .Include(e => e.Aws)
                .FirstOrDefaultAsync(e => e.Id == employeeId);
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

        public async Task<List<EmployeeListItem>> SearchAsync(
    string? query,
    bool activeOnly,
    string? manager = null,
    string? supervisor = null,
    string? org = null,
    string? subOrg = null,
    TimeDisplayMode mode = TimeDisplayMode.Eastern,
    int take = 1000,
    CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var baseQuery = db.EmployeeCurrentDetails.AsQueryable();

            if (activeOnly) baseQuery = baseQuery.Where(x => x.IsActive);

            if (!string.IsNullOrEmpty(manager)) baseQuery = baseQuery.Where(x => x.ManagerName == manager);
            if (!string.IsNullOrEmpty(supervisor)) baseQuery = baseQuery.Where(x => x.SupervisorName == supervisor);
            if (!string.IsNullOrEmpty(org)) baseQuery = baseQuery.Where(x => x.OrganizationName == org);
            if (!string.IsNullOrEmpty(subOrg)) baseQuery = baseQuery.Where(x => x.SubOrganizationName == subOrg);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim();
                var isId = int.TryParse(q, out var idValue);

                baseQuery = baseQuery.Where(x =>
                    // 1. Match "Last, First MI" (e.g. "Doe, John M" or "Doe, John")
                    (x.LastName + ", " + x.FirstName + (x.MiddleInitial == null ? "" : " " + x.MiddleInitial)).Contains(q)
                    ||
                    // 2. Match "First Last" (e.g. "John Doe")
                    (x.FirstName + " " + x.LastName).Contains(q)
                    ||
                    // 3. Match individual parts or ID
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
                    MiddleInitial = x.MiddleInitial,
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
                    item.Schedule = FormatSchedule(sched, mode);
                    item.DailySchedules = BuildDailySchedules(sched, mode);
                }
            }

            return items;
        }

        public async Task<EmployeeFilterOptionsDto> GetFilterOptionsAsync()
        {
            using var ctx = await _factory.CreateDbContextAsync();

            var managers = await ctx.Managers.AsNoTracking()
                .Where(x => x.IsActive == true)
                .Select(x => ctx.Employees.Where(e => e.Id == x.EmployeeId)
                                          .Select(e => e.LastName + ", " + e.FirstName)
                                          .FirstOrDefault())
                .Where(x => x != null)
                .OrderBy(x => x)
                .ToListAsync();

            var supervisors = await ctx.Supervisors.AsNoTracking()
                .Where(x => x.IsActive == true)
                .Select(x => ctx.Employees.Where(e => e.Id == x.EmployeeId)
                                          .Select(e => e.LastName + ", " + e.FirstName)
                                          .FirstOrDefault())
                .Where(x => x != null)
                .OrderBy(x => x)
                .ToListAsync();

            var orgs = await ctx.Organizations.AsNoTracking()
                .Where(x => x.IsActive == true && x.Name != null)
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToListAsync();

            var subOrgs = await ctx.SubOrganizations.AsNoTracking()
                .Where(x => x.IsActive == true && x.Name != null)
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToListAsync();

            return new EmployeeFilterOptionsDto
            {
                Managers = managers!,
                Supervisors = supervisors!,
                Organizations = orgs!,
                SubOrganizations = subOrgs!
            };
        }
        public async Task UpdateProfileAsync(EmployeeProfileUpdateDto dto)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            // Include Aws to check current state if needed
            var emp = await ctx.Employees
                .Include(e => e.Aws)
                .FirstOrDefaultAsync(e => e.Id == dto.Id);

            if (emp != null)
            {
                // Update standard profile fields
                emp.FirstName = dto.FirstName;
                emp.LastName = dto.LastName;
                emp.MiddleInitial = dto.MiddleInitial;
                emp.NmciEmail = dto.NmciEmail;
                emp.UsnOperatorId = dto.UsnOperatorId;
                emp.UsnAdminId = dto.UsnAdminId;
                emp.CorporateEmail = dto.CorporateEmail;
                emp.CorporateId = dto.CorporateId;
                emp.DomainLoginName = dto.DomainLoginName;

                // --- FIX: Handle AWS Relationship Sync ---
                // If the selection has changed (or if we need to enforce self-healing)
                if (emp.AwsId != dto.AwsId)
                {
                    // 1. Unlink any Identifier currently pointing to this employee
                    // We query Identifiers directly because emp.Aws might be null if data is inconsistent
                    var oldLinks = await ctx.Identifiers.Where(i => i.EmployeeId == emp.Id).ToListAsync();
                    foreach (var link in oldLinks)
                    {
                        link.EmployeeId = null;
                    }

                    // 2. Link the new Identifier (if one is selected)
                    if (dto.AwsId.HasValue)
                    {
                        var newLink = await ctx.Identifiers.FindAsync(dto.AwsId.Value);
                        if (newLink != null)
                        {
                            newLink.EmployeeId = emp.Id;
                        }
                    }

                    // 3. Update the local column on Employees (to keep the bidirectional ref in sync)
                    emp.AwsId = dto.AwsId;
                }
                else if (dto.AwsId.HasValue && emp.Aws == null)
                {
                    // Edge Case: AWS ID is set on Employee, but the Identifier doesn't point back (Data Mismatch)
                    var link = await ctx.Identifiers.FindAsync(dto.AwsId.Value);
                    if (link != null && link.EmployeeId != emp.Id)
                    {
                        link.EmployeeId = emp.Id;
                    }
                }

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
        // UPDATED: GetDailySchedulesAsync 
        // - Uses Subtractive Logic for Valid Work Windows
        // - Merges Actuals to handle short segments
        // - Clips future alerts based on "Now" in target timezone
        // ====================================================================
        public async Task<ScheduleDashboardVm> GetDailySchedulesAsync(
            DateOnly date,
            TimeDisplayMode mode,
            string? query,
            bool activeOnly,
            bool agentsOnly,
            string? manager,
            string? supervisor,
            string? org,
            string? subOrg,
            CancellationToken ct = default)
        {
            if (date == DateOnly.MinValue) return new ScheduleDashboardVm { Date = date };

            using var ctx = await _factory.CreateDbContextAsync();

            var rawData = await ctx.DetailedSchedule.AsNoTracking()
                .Where(x => x.ScheduleDate == date)
                .Include(x => x.ActivityType)
                .Include(x => x.ActivitySubType)
                .Include(x => x.AwsStatus)
                .ToListAsync(ct);

            var empIds = rawData.Select(x => x.EmployeeId).Distinct().ToList();
            var histories = await ctx.EmployeeHistory.AsNoTracking()
                .Where(h => empIds.Contains(h.EmployeeId))
                .Include(h => h.Site)
                .Include(h => h.Organization)
                .Include(h => h.SubOrganization)
                .Include(h => h.Supervisor).ThenInclude(s => s.Employee)
                .Include(h => h.Manager).ThenInclude(m => m.Employee)
                .Include(h => h.Employee)
                .ToListAsync(ct);

            var bestHistory = histories
                .GroupBy(h => h.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.IsActive == true).ThenByDescending(x => x.EffectiveDate).First())
                .ToList();

            var filteredHistory = bestHistory.Where(h =>
            {
                if (activeOnly && h.IsActive != true) return false;

                if (agentsOnly)
                {
                    if (h.SubOrganization?.AwsStatusId == null || h.SubOrganization.AwsStatusId == 1) return false;
                }

                if (!string.IsNullOrEmpty(manager))
                {
                    var mName = h.Manager?.Employee != null ? $"{h.Manager.Employee.LastName}, {h.Manager.Employee.FirstName}" : "";
                    if (!mName.Contains(manager, StringComparison.OrdinalIgnoreCase)) return false;
                }

                if (!string.IsNullOrEmpty(supervisor))
                {
                    var sName = h.Supervisor?.Employee != null ? $"{h.Supervisor.Employee.LastName}, {h.Supervisor.Employee.FirstName}" : "";
                    if (!sName.Contains(supervisor, StringComparison.OrdinalIgnoreCase)) return false;
                }

                if (!string.IsNullOrEmpty(org) && h.Organization?.Name != org) return false;
                if (!string.IsNullOrEmpty(subOrg) && h.SubOrganization?.Name != subOrg) return false;

                return true;
            }).ToList();

            var visibleEmpIds = filteredHistory.Select(x => x.EmployeeId).Distinct().ToList();
            var awsMappingsRaw = await ctx.Identifiers.AsNoTracking()
                .Where(x => visibleEmpIds.Contains(x.EmployeeId ?? 0) && !string.IsNullOrEmpty(x.Guid))
                .Select(x => new { x.EmployeeId, x.Guid })
                .ToListAsync(ct);

            var guidToEmpMap = new Dictionary<Guid, int>();
            foreach (var row in awsMappingsRaw)
            {
                if (row.EmployeeId.HasValue && Guid.TryParse(row.Guid, out Guid parsedGuid))
                    if (!guidToEmpMap.ContainsKey(parsedGuid)) guidToEmpMap[parsedGuid] = row.EmployeeId.Value;
            }
            var distinctGuids = guidToEmpMap.Keys.ToList();

            var sqlQueryStart = date.AddDays(-1).ToDateTime(TimeOnly.MinValue);
            var sqlQueryEnd = date.AddDays(2).ToDateTime(TimeOnly.MinValue);

            List<AwsAgentActivityDto> awsActivities = new();
            if (distinctGuids.Any())
            {
                awsActivities = await GetAwsActivitiesAsync(ctx, distinctGuids, sqlQueryStart, sqlQueryEnd, ct);
            }

            var employeeAwsData = awsActivities
                .Where(a => guidToEmpMap.ContainsKey(a.AwsGuid))
                .ToLookup(a => guidToEmpMap[a.AwsGuid]);

            var dashboard = new ScheduleDashboardVm { Date = date };

            var globalDayStart = date.ToDateTime(TimeOnly.MinValue);
            var globalDayEnd = globalDayStart.AddHours(30);

            var groupedBySite = filteredHistory.GroupBy(x => x.Site?.SiteCode ?? "Unknown").OrderBy(g => g.Key);

            foreach (var siteGroup in groupedBySite)
            {
                var siteName = siteGroup.Key;
                var first = siteGroup.First();
                var siteTz = first.Site?.TimeZoneWindows ?? first.Site?.TimeZoneIana ?? "Eastern Standard Time";
                var siteVm = new SiteScheduleGroupVm { SiteName = siteName, TimeZoneId = siteTz };

                string targetTzId = mode switch
                {
                    TimeDisplayMode.Eastern => "Eastern Standard Time",
                    TimeDisplayMode.Mountain => "Mountain Standard Time",
                    _ => siteTz
                };

                // Determine "Now" cutoff in target Timezone to prevent future alerts
                DateTime nowInTarget;
                try
                {
                    var targetZone = TimeZoneInfo.FindSystemTimeZoneById(targetTzId);
                    nowInTarget = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, targetZone);
                }
                catch
                {
                    nowInTarget = DateTime.MaxValue; // Fail safe
                }

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

                    DateTime validWindowStartEt;
                    DateTime validWindowEndEt;

                    if (empSchedules.Any())
                    {
                        var minStart = empSchedules.Min(s => s.StartTime);
                        var maxEnd = empSchedules.Max(s => s.EndTime);
                        validWindowStartEt = minStart.AddHours(-8);
                        validWindowEndEt = maxEnd.AddHours(8);
                    }
                    else
                    {
                        validWindowStartEt = Tz.FromSiteToEt(globalDayStart, targetTzId);
                        validWindowEndEt = Tz.FromSiteToEt(globalDayEnd, targetTzId);
                    }

                    // Prepare for Alert Logic: 
                    var actualSegmentsForAlerts = new List<(DateTime Start, DateTime End)>();
                    var offlineSegments = new List<(DateTime Start, DateTime End)>();
                    var workingSegments = new List<(DateTime Start, DateTime End)>();

                    // 1. Classify Schedule Segments
                    foreach (var s in empSchedules)
                    {
                        var sStart = Tz.FromEtToSite(s.StartTime, targetTzId);
                        var sEnd = Tz.FromEtToSite(s.EndTime, targetTzId);

                        // If ID=1, it is an Offline Segment (Exceptions, Breaks, etc.)
                        if (s.AwsStatus?.Id == 1)
                        {
                            offlineSegments.Add((sStart, sEnd));
                        }
                        else
                        {
                            // Otherwise, it is a Working Segment (Shift, Overtime, etc.)
                            workingSegments.Add((sStart, sEnd));
                        }
                    }

                    // 2. Build Valid Work Windows (Working - Offline)
                    // This handles overlapping segments where an offline activity overrides a shift
                    var validWorkWindows = new List<(DateTime Start, DateTime End)>();

                    if (workingSegments.Any())
                    {
                        // Start with working segments sorted
                        workingSegments.Sort((a, b) => a.Start.CompareTo(b.Start));
                        offlineSegments.Sort((a, b) => a.Start.CompareTo(b.Start));

                        foreach (var work in workingSegments)
                        {
                            var currentStart = work.Start;
                            var workEnd = work.End;

                            // Subtract overlapping offline segments
                            foreach (var off in offlineSegments)
                            {
                                if (off.End <= currentStart) continue;
                                if (off.Start >= workEnd) break;

                                // Valid Work before the offline segment starts
                                if (off.Start > currentStart)
                                {
                                    validWorkWindows.Add((currentStart, off.Start));
                                }

                                // Advance start to end of offline segment
                                if (off.End > currentStart) currentStart = off.End;
                            }

                            // Remaining work after last offline segment
                            if (currentStart < workEnd)
                            {
                                validWorkWindows.Add((currentStart, workEnd));
                            }
                        }
                    }

                    // A. Map Scheduled Segments & Build View
                    DateTime? minDt = null;
                    DateTime? maxDt = null;

                    foreach (var item in empSchedules)
                    {
                        var localStart = Tz.FromEtToSite(item.StartTime, targetTzId);
                        var localEnd = Tz.FromEtToSite(item.EndTime, targetTzId);

                        if (minDt == null || localStart < minDt) minDt = localStart;
                        if (maxDt == null || localEnd > maxDt) maxDt = localEnd;

                        empVm.Segments.Add(new ScheduleSegmentVm
                        {
                            ActivityTypeId = item.ActivityTypeId,
                            ActivitySubTypeId = item.ActivitySubTypeId,
                            ActivityName = item.ActivityType?.Name ?? "?",
                            SubActivityName = item.ActivitySubType?.Name ?? "-",
                            AwsStatusId = item.AwsStatus?.Id,
                            AwsStatusName = item.AwsStatus?.Name ?? "-",
                            Start = localStart,
                            End = localEnd,
                            IsImpacting = item.IsImpacting ?? false
                        });
                    }
                    empVm.ShiftString = (minDt.HasValue && maxDt.HasValue) ? $"{minDt.Value:HH:mm} - {maxDt.Value:HH:mm}" : "OFF";

                    // B. Map Actual Segments (AWS)
                    if (employeeAwsData.Contains(hist.EmployeeId))
                    {
                        foreach (var act in employeeAwsData[hist.EmployeeId])
                        {
                            if (act.StartTime >= validWindowStartEt && act.StartTime < validWindowEndEt)
                            {
                                var actStart = Tz.FromEtToSite(act.StartTime, targetTzId);
                                var actEnd = Tz.FromEtToSite(act.EndTime, targetTzId);

                                empVm.AwsSegments.Add(new ScheduleSegmentVm
                                {
                                    ActivityName = "AWS",
                                    SubActivityName = act.CurrentAgentStatus,
                                    AwsStatusName = act.CurrentAgentStatus,
                                    Start = actStart,
                                    End = actEnd,
                                    IsImpacting = true
                                });

                                actualSegmentsForAlerts.Add((actStart, actEnd));
                            }
                        }
                    }

                    // MERGE ACTUALS
                    var mergedActuals = new List<(DateTime Start, DateTime End)>();
                    if (actualSegmentsForAlerts.Any())
                    {
                        actualSegmentsForAlerts.Sort((a, b) => a.Start.CompareTo(b.Start));
                        var currentStart = actualSegmentsForAlerts[0].Start;
                        var currentEnd = actualSegmentsForAlerts[0].End;

                        for (int i = 1; i < actualSegmentsForAlerts.Count; i++)
                        {
                            if (actualSegmentsForAlerts[i].Start <= currentEnd.AddMinutes(1))
                            {
                                if (actualSegmentsForAlerts[i].End > currentEnd) currentEnd = actualSegmentsForAlerts[i].End;
                            }
                            else
                            {
                                mergedActuals.Add((currentStart, currentEnd));
                                currentStart = actualSegmentsForAlerts[i].Start;
                                currentEnd = actualSegmentsForAlerts[i].End;
                            }
                        }
                        mergedActuals.Add((currentStart, currentEnd));
                    }

                    // -----------------------------------------------------------
                    // ALERT LOGIC 1: Missing Work (> 5 minutes)
                    // (Schedule says Work, Actual is Missing)
                    // -----------------------------------------------------------
                    if (empVm.Segments.Any())
                    {
                        var subtractMask = new List<(DateTime Start, DateTime End)>();
                        subtractMask.AddRange(mergedActuals);
                        subtractMask.AddRange(offlineSegments); // Also subtract authorized offline times
                        subtractMask.Sort((a, b) => a.Start.CompareTo(b.Start));

                        foreach (var plan in empVm.Segments)
                        {
                            if (plan.AwsStatusId == 1) continue;

                            var current = plan.Start;
                            var end = plan.End;

                            foreach (var mask in subtractMask)
                            {
                                if (mask.End <= current) continue;
                                if (mask.Start >= end) break;

                                // Gap detected
                                if (mask.Start > current)
                                {
                                    var alertStart = current;
                                    var alertEnd = mask.Start;

                                    // CLIP FUTURE ALERTS
                                    if (alertStart < nowInTarget)
                                    {
                                        if (alertEnd > nowInTarget) alertEnd = nowInTarget;

                                        if ((alertEnd - alertStart).TotalMinutes > 5)
                                        {
                                            empVm.AlertSegments.Add(new ScheduleSegmentVm
                                            {
                                                ActivityName = "Alert",
                                                SubActivityName = "Missing",
                                                Start = alertStart,
                                                End = alertEnd,
                                                IsImpacting = true
                                            });
                                        }
                                    }
                                }
                                if (mask.End > current) current = mask.End;
                            }

                            if (current < end)
                            {
                                var alertStart = current;
                                var alertEnd = end;

                                // CLIP FUTURE ALERTS
                                if (alertStart < nowInTarget)
                                {
                                    if (alertEnd > nowInTarget) alertEnd = nowInTarget;

                                    if ((alertEnd - alertStart).TotalMinutes > 5)
                                    {
                                        empVm.AlertSegments.Add(new ScheduleSegmentVm
                                        {
                                            ActivityName = "Alert",
                                            SubActivityName = "Missing",
                                            Start = alertStart,
                                            End = alertEnd,
                                            IsImpacting = true
                                        });
                                    }
                                }
                            }
                        }
                    }

                    // -----------------------------------------------------------
                    // ALERT LOGIC 2: Unexpected Work (> 5 minutes)
                    // (Actual exists, but Valid Work Window is missing)
                    // -----------------------------------------------------------
                    if (mergedActuals.Any())
                    {
                        foreach (var act in mergedActuals)
                        {
                            var current = act.Start;
                            var end = act.End;

                            // Subtract VALID WORK windows from Actuals
                            // If actual remains, it is unexpected
                            foreach (var valid in validWorkWindows)
                            {
                                if (valid.End <= current) continue;
                                if (valid.Start >= end) break;

                                // Unexpected Work
                                if (valid.Start > current)
                                {
                                    var alertStart = current;
                                    var alertEnd = valid.Start;

                                    // CLIP FUTURE ALERTS
                                    if (alertStart < nowInTarget)
                                    {
                                        if (alertEnd > nowInTarget) alertEnd = nowInTarget;

                                        if ((alertEnd - alertStart).TotalMinutes > 5)
                                        {
                                            empVm.AlertSegments.Add(new ScheduleSegmentVm
                                            {
                                                ActivityName = "Alert",
                                                SubActivityName = "Unexpected",
                                                Start = alertStart,
                                                End = alertEnd,
                                                IsImpacting = true
                                            });
                                        }
                                    }
                                }
                                if (valid.End > current) current = valid.End;
                            }

                            if (current < end)
                            {
                                var alertStart = current;
                                var alertEnd = end;

                                // CLIP FUTURE ALERTS
                                if (alertStart < nowInTarget)
                                {
                                    if (alertEnd > nowInTarget) alertEnd = nowInTarget;

                                    if ((alertEnd - alertStart).TotalMinutes > 5)
                                    {
                                        empVm.AlertSegments.Add(new ScheduleSegmentVm
                                        {
                                            ActivityName = "Alert",
                                            SubActivityName = "Unexpected",
                                            Start = alertStart,
                                            End = alertEnd,
                                            IsImpacting = true
                                        });
                                    }
                                }
                            }
                        }
                    }

                    siteVm.Employees.Add(empVm);
                }

                siteVm.Employees = siteVm.Employees.OrderBy(e => e.EmployeeName).ToList();
                if (siteVm.Employees.Any()) dashboard.Sites.Add(siteVm);
            }

            return dashboard;
        }

        public async Task<EmployeeDetailDto?> GetDetailAsync(int employeeId, CancellationToken ct = default)
        {
            return null;
        }

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

        private async Task<List<AwsAgentActivityDto>> GetAwsActivitiesAsync(
            AomDbContext db,
            List<Guid> visibleEmployeeAwsGuids,
            DateTime viewStart,
            DateTime viewEnd,
            CancellationToken ct = default)
        {
            if (!visibleEmployeeAwsGuids.Any()) return new List<AwsAgentActivityDto>();

            var guidList = string.Join("','", visibleEmployeeAwsGuids);

            // Fetch only Online activities (Not 'Offline')
            var sql = $@"
    SELECT CAST([eventId] AS NVARCHAR(36)) as [EventId]
          ,CAST([awsId] AS NVARCHAR(36)) as [AwsId]
          ,[eventTimeET] as [StartTime]
          ,[currentAgentStatus]
          ,[endTime] as [EndTime]
          ,[duration]
          ,CAST([AwsGuid] AS UNIQUEIDENTIFIER) as [AwsGuid]
      FROM [TODNMCIAWS].[dbo].[awsDetailedAgentData]
      WHERE [AwsGuid] IN ('{guidList}')
        AND [eventTimeET] < {{0}} 
        AND [endTime] > {{1}}
        AND [currentAgentStatus] != 'Offline'
      ORDER BY [eventTimeET]";

            return await db.Set<AwsAgentActivityDto>()
                .FromSqlRaw(sql, viewEnd, viewStart)
                .AsNoTracking()
                .ToListAsync(ct);
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