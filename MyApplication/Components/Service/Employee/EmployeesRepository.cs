using Microsoft.EntityFrameworkCore;
using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using System.Linq.Expressions;

namespace MyApplication.Components.Service.Employee
{
    public sealed class EmployeesRepository
    {
        private readonly IDbContextFactory<AomDbContext> _contextFactory;

        public EmployeesRepository(IDbContextFactory<AomDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<EmployeeFullDetailsVm> GetFullDetailsAsync(int employeeId)
        {
            using var ctx = await _contextFactory.CreateDbContextAsync();

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



        public async Task<EmployeeFilterOptionsDto> GetFilterOptionsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var managers = await context.Managers
                .Where(x => x.IsActive == true)
                .Select(x => x.Employee.LastName + ", " + x.Employee.FirstName)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var supervisors = await context.Supervisors
                .Where(x => x.IsActive == true)
                .Select(x => x.Employee.LastName + ", " + x.Employee.FirstName)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var orgs = await context.Organizations
                .Where(x => x.IsActive == true)
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var subOrgs = await context.SubOrganizations
                .Where(x => x.IsActive == true)
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return new EmployeeFilterOptionsDto
            {
                Managers = managers,
                Supervisors = supervisors,
                Organizations = orgs,
                SubOrganizations = subOrgs
            };
        }

        public async Task<List<EmployeeDto>> SearchAsync(string searchText, bool activeOnly = true)
        {
            return await GetEmployeesAsync(new EmployeesFilterDto { SearchText = searchText, IncludeInactive = !activeOnly });
        }

        public async Task<List<EmployeeListItem>> SearchAsync(string searchText, bool activeOnly, object? mode, int take, CancellationToken ct)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // Construct query using EmployeeHistory to find the latest record for each employee
            var latestHistoryIds = context.EmployeeHistory
                .GroupBy(h => h.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.EffectiveDate).Select(x => x.Id).FirstOrDefault());

            var query = context.EmployeeHistory
                .Where(h => latestHistoryIds.Contains(h.Id))
                .Include(x => x.Employee)
                .Include(x => x.Organization)
                .Include(x => x.SubOrganization)
                .Include(x => x.Supervisor).ThenInclude(s => s.Employee)
                .Include(x => x.Manager).ThenInclude(m => m.Employee)
                .Include(x => x.ScheduleRequest).ThenInclude(s => s.AcrSchedules)
                .AsQueryable();

            if (activeOnly)
            {
                query = query.Where(x => x.Employee.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var terms = searchText.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var term in terms)
                {
                    query = query.Where(x => x.Employee.FirstName.Contains(term) ||
                                             x.Employee.LastName.Contains(term) ||
                                             x.Employee.MiddleInitial.Contains(term) ||
                                             x.Employee.Id.ToString().Contains(term));
                }
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            var items = await query.ToListAsync(ct); // Materialize to fetch related entities efficiently

            var empIds = items.Select(x => x.EmployeeId).ToList();
            var routingProfiles = await context.EmployeeRoutingProfiles
                .Where(rp => empIds.Contains(rp.EmployeeId))
                .Include(rp => rp.WeekdayProfile)
                .Include(rp => rp.WeekendProfile)
                .ToListAsync(ct);

            var skills = await context.Skills
                .Where(s => empIds.Contains(s.EmployeeId) && s.IsActive == true)
                .Include(s => s.SkillType)
                .ToListAsync(ct);

            var result = new List<EmployeeListItem>();

            foreach (var h in items)
            {
                var rp = routingProfiles.FirstOrDefault(p => p.EmployeeId == h.EmployeeId);
                var empSkills = skills.Where(s => s.EmployeeId == h.EmployeeId).Select(s => s.SkillType.Name).ToList();

                string scheduleStr = "-";
                if (h.ScheduleRequest != null && h.ScheduleRequest.AcrSchedules != null)
                {
                    // Assuming mode is passed as TimeDisplayMode, let's cast it or default to Eastern
                    var timeMode = mode is TimeDisplayMode tm ? tm : TimeDisplayMode.Eastern;
                    scheduleStr = FormatSchedule(h.ScheduleRequest.AcrSchedules.FirstOrDefault(s => s.ShiftNumber == 1), timeMode);
                }

                result.Add(new EmployeeListItem
                {
                    Id = h.EmployeeId,
                    FirstName = h.Employee.FirstName,
                    LastName = h.Employee.LastName,
                    MiddleInitial = h.Employee.MiddleInitial,
                    IsActive = h.Employee.IsActive,
                    Organization = h.Organization != null ? h.Organization.Name : null,
                    SubOrganization = h.SubOrganization != null ? h.SubOrganization.Name : null,
                    Supervisor = h.Supervisor != null ? $"{h.Supervisor.Employee.LastName}, {h.Supervisor.Employee.FirstName}" : null,
                    Manager = h.Manager != null ? $"{h.Manager.Employee.LastName}, {h.Manager.Employee.FirstName}" : null,
                    Skills = empSkills,
                    WeekdayProfile = rp?.WeekdayProfile?.Name,
                    WeekendProfile = rp?.WeekendProfile?.Name,
                    Schedule = scheduleStr
                });
            }

            return result;
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync(EmployeesFilterDto filter)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var latestHistoryIds = context.EmployeeHistory
                .GroupBy(h => h.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.EffectiveDate).Select(x => x.Id).FirstOrDefault());

            var query = context.EmployeeHistory
                .Where(h => latestHistoryIds.Contains(h.Id))
                .Include(x => x.Employee)
                .Include(x => x.Organization)
                .Include(x => x.SubOrganization)
                .Include(x => x.Site)
                .Include(x => x.Supervisor).ThenInclude(s => s.Employee)
                .Include(x => x.Manager).ThenInclude(m => m.Employee)
                .AsQueryable();

            if (!filter.IncludeInactive)
            {
                query = query.Where(x => x.Employee.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                var terms = filter.SearchText.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var term in terms)
                {
                    query = query.Where(x => x.Employee.FirstName.Contains(term) ||
                                             x.Employee.LastName.Contains(term) ||
                                             x.Employee.MiddleInitial.Contains(term) ||
                                             x.Employee.Id.ToString().Contains(term));
                }
            }

            if (filter.OrganizationIds != null && filter.OrganizationIds.Any())
            {
                query = query.Where(x => x.OrganizationId.HasValue && filter.OrganizationIds.Contains(x.OrganizationId.Value));
            }

            if (filter.SubOrganizationIds != null && filter.SubOrganizationIds.Any())
            {
                query = query.Where(x => x.SubOrganizationId.HasValue && filter.SubOrganizationIds.Contains(x.SubOrganizationId.Value));
            }

            if (filter.SiteIds != null && filter.SiteIds.Any())
            {
                query = query.Where(x => x.SiteId.HasValue && filter.SiteIds.Contains(x.SiteId.Value));
            }

            if (filter.SupervisorIds != null && filter.SupervisorIds.Any())
            {
                query = query.Where(x => x.SupervisorId.HasValue && filter.SupervisorIds.Contains(x.SupervisorId.Value));
            }

            if (filter.ManagerIds != null && filter.ManagerIds.Any())
            {
                query = query.Where(x => x.ManagerId.HasValue && filter.ManagerIds.Contains(x.ManagerId.Value));
            }

            var items = await query.ToListAsync();
            var empIds = items.Select(x => x.EmployeeId).ToList();
            var skills = await context.Skills
                .Where(s => empIds.Contains(s.EmployeeId) && s.IsActive == true)
                .Include(s => s.SkillType)
                .ToListAsync();

            return items.Select(x =>
            {
                var fullName = $"{x.Employee.LastName}, {x.Employee.FirstName}";
                if (!string.IsNullOrEmpty(x.Employee.MiddleInitial)) fullName += $" {x.Employee.MiddleInitial}";
                fullName += $" ({x.Employee.Id})";

                return new EmployeeDto
                {
                    Id = x.EmployeeId,
                    Name = fullName,
                    Organization = x.Organization?.Name,
                    OrganizationId = x.OrganizationId ?? 0,
                    SubOrganization = x.SubOrganization?.Name,
                    SubOrganizationId = x.SubOrganizationId ?? 0,
                    Site = x.Site?.SiteCode,
                    SiteId = x.SiteId ?? 0,
                    Supervisor = x.Supervisor != null ? $"{x.Supervisor.Employee.LastName}, {x.Supervisor.Employee.FirstName}" : null,
                    SupervisorId = x.SupervisorId,
                    Manager = x.Manager != null ? $"{x.Manager.Employee.LastName}, {x.Manager.Employee.FirstName}" : null,
                    ManagerId = x.ManagerId,
                    IsActive = x.Employee.IsActive,
             
                    Skills = skills.Where(s => s.EmployeeId == x.EmployeeId).Select(s => s.SkillType.Name).ToList()
                };
            }).ToList();
        }

        public async Task<List<OrganizationDto>> GetOrganizationsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Organizations
                .Where(x => x.IsActive == true)
                .Select(x => new OrganizationDto { Id = x.Id, Name = x.Name })
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<SubOrganizationDto>> GetSubOrganizationsAsync(IEnumerable<int>? organizationIds = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var query = context.SubOrganizations
                .Where(x => x.IsActive == true);

            if (organizationIds != null && organizationIds.Any())
            {
                query = query.Where(x => x.OrganizationId == null || organizationIds.Contains(x.OrganizationId.Value));
            }

            return await query
                .Select(x => new SubOrganizationDto
                {
                    Id = x.Id,
                    OrganizationId = x.OrganizationId ?? 0,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<SiteDto>> GetSitesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Sites
                .Where(x => x.IsActive == true)
                .Select(x => new SiteDto { Id = x.Id, Name = x.SiteCode })
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<SupervisorDto>> GetSupervisorsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Supervisors
                .Where(x => x.IsActive == true)
                .Select(x => new SupervisorDto { Id = x.Id, Name = x.Employee.LastName + ", " + x.Employee.FirstName })
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<ManagerDto>> GetManagersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Managers
                .Where(x => x.IsActive == true)
                .Select(x => new ManagerDto { Id = x.Id, Name = x.Employee.LastName + ", " + x.Employee.FirstName })
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task UpdateProfileAsync(EmployeeProfileUpdateDto dto)
        {
            using var ctx = await _contextFactory.CreateDbContextAsync();
            var emp = await ctx.Employees
                .Include(e => e.Aws)
                .FirstOrDefaultAsync(e => e.Id == dto.Id);

            if (emp != null)
            {
                emp.FirstName = dto.FirstName;
                emp.LastName = dto.LastName;
                emp.MiddleInitial = dto.MiddleInitial;
                emp.NmciEmail = dto.NmciEmail;
                emp.UsnOperatorId = dto.UsnOperatorId;
                emp.UsnAdminId = dto.UsnAdminId;
                emp.CorporateEmail = dto.CorporateEmail;
                emp.CorporateId = dto.CorporateId;
                emp.DomainLoginName = dto.DomainLoginName;

                if (emp.AwsId != dto.AwsId)
                {
                    var oldLinks = await ctx.Identifiers.Where(i => i.EmployeeId == emp.Id).ToListAsync();
                    foreach (var link in oldLinks)
                    {
                        link.EmployeeId = null;
                    }

                    if (dto.AwsId.HasValue)
                    {
                        var newLink = await ctx.Identifiers.FindAsync(dto.AwsId.Value);
                        if (newLink != null)
                        {
                            newLink.EmployeeId = emp.Id;
                        }
                    }

                    emp.AwsId = dto.AwsId;
                }
                else if (dto.AwsId.HasValue && emp.Aws == null)
                {
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
            using var ctx = await _contextFactory.CreateDbContextAsync(ct);
            return await ctx.Identifiers.AsNoTracking()
                .Where(x => EF.Functions.Like(x.AwsUsername, $"%{query}%"))
                .Take(20)
                .Select(x => new AwsIdentifierLookupDto { Id = x.Id, AwsUsername = x.AwsUsername })
                .ToListAsync(ct);
        }

        public async Task<AwsIdentifierLookupDto?> GetAwsIdentifierAsync(int id)
        {
            using var ctx = await _contextFactory.CreateDbContextAsync();
            return await ctx.Identifiers.Where(x => x.Id == id)
               .Select(x => new AwsIdentifierLookupDto { Id = x.Id, AwsUsername = x.AwsUsername })
               .FirstOrDefaultAsync();
        }

        public async Task<ScheduleDashboardVm> GetDailySchedulesAsync(
            DateOnly date,
            TimeDisplayMode mode,
            string? query,
            bool activeOnly,
            bool agentsOnly,
            bool hideLoa,
            string? manager,
            string? supervisor,
            string? org,
            string? subOrg,
            CancellationToken ct = default)
        {
            if (date == DateOnly.MinValue) return new ScheduleDashboardVm { Date = date };

            using var ctx = await _contextFactory.CreateDbContextAsync();

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
                if (hideLoa && (h.IsLoa == true || h.IsIntLoa == true)) return false;

                if (agentsOnly)
                {
                    var allowedOrgIds = new[] { 2, 3, 4 };
                    if (h.Organization == null || !allowedOrgIds.Contains(h.Organization.Id)) return false;

                    var excludedSubOrgIds = new[] { 2, 3 };
                    if (h.SubOrganization != null && excludedSubOrgIds.Contains(h.SubOrganization.Id)) return false;
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
            var groupedBySite = filteredHistory.GroupBy(x => x.Site?.SiteCode ?? "Unknown").OrderBy(g => g.Key);

            var globalDayStart = date.ToDateTime(TimeOnly.MinValue);
            var globalDayEnd = globalDayStart.AddHours(30);

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

                DateTime nowInTarget;
                try
                {
                    var targetZone = TimeZoneInfo.FindSystemTimeZoneById(targetTzId);
                    nowInTarget = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, targetZone);
                }
                catch { nowInTarget = DateTime.MaxValue; }

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

                    var actualSegmentsForAlerts = new List<(DateTime Start, DateTime End)>();
                    var offlineSegments = new List<(DateTime Start, DateTime End)>();
                    var workingSegments = new List<(DateTime Start, DateTime End)>();

                    foreach (var s in empSchedules)
                    {
                        var sStart = Tz.FromEtToSite(s.StartTime, targetTzId);
                        var sEnd = Tz.FromEtToSite(s.EndTime, targetTzId);

                        if (s.AwsStatus?.Id == 1) offlineSegments.Add((sStart, sEnd));
                        else workingSegments.Add((sStart, sEnd));
                    }

                    var validWorkWindows = new List<(DateTime Start, DateTime End)>();
                    if (workingSegments.Any())
                    {
                        workingSegments.Sort((a, b) => a.Start.CompareTo(b.Start));
                        offlineSegments.Sort((a, b) => a.Start.CompareTo(b.Start));

                        foreach (var work in workingSegments)
                        {
                            var currentStart = work.Start;
                            var workEnd = work.End;

                            foreach (var off in offlineSegments)
                            {
                                if (off.End <= currentStart) continue;
                                if (off.Start >= workEnd) break;
                                if (off.Start > currentStart) validWorkWindows.Add((currentStart, off.Start));
                                if (off.End > currentStart) currentStart = off.End;
                            }
                            if (currentStart < workEnd) validWorkWindows.Add((currentStart, workEnd));
                        }
                    }

                    DateTime? minDt = null;
                    DateTime? maxDt = null;

                    foreach (var item in empSchedules)
                    {
                        var localStart = Tz.FromEtToSite(item.StartTime, targetTzId);
                        var localEnd = Tz.FromEtToSite(item.EndTime, targetTzId);

                        if (minDt == null || localStart < minDt) minDt = localStart;
                        if (maxDt == null || localEnd > maxDt) maxDt = localEnd;

                        string colorClass = GetActivityColor(
                            item.ActivityType?.Name,
                            item.ActivitySubType?.Name,
                            item.AwsStatus?.Name);

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
                            IsImpacting = item.IsImpacting ?? false,
                            ColorClass = colorClass
                        });
                    }
                    empVm.ShiftString = (minDt.HasValue && maxDt.HasValue) ? $"{minDt.Value:HH:mm} - {maxDt.Value:HH:mm}" : "OFF";

                    if (employeeAwsData.Contains(hist.EmployeeId))
                    {
                        foreach (var act in employeeAwsData[hist.EmployeeId])
                        {
                            if (act.StartTime >= validWindowStartEt && act.StartTime < validWindowEndEt)
                            {
                                var actStart = Tz.FromEtToSite(act.StartTime, targetTzId);
                                var actEnd = Tz.FromEtToSite(act.EndTime, targetTzId);

                                string awsColor = GetActivityColor(null, null, act.CurrentAgentStatus);

                                empVm.AwsSegments.Add(new ScheduleSegmentVm
                                {
                                    ActivityName = "AWS",
                                    SubActivityName = act.CurrentAgentStatus,
                                    AwsStatusName = act.CurrentAgentStatus,
                                    Start = actStart,
                                    End = actEnd,
                                    IsImpacting = true,
                                    ColorClass = awsColor
                                });

                                actualSegmentsForAlerts.Add((actStart, actEnd));
                            }
                        }
                    }

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

                    if (empVm.Segments.Any())
                    {
                        var subtractMask = new List<(DateTime Start, DateTime End)>();
                        subtractMask.AddRange(mergedActuals);
                        subtractMask.AddRange(offlineSegments);
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
                                if (mask.Start > current)
                                {
                                    var alertStart = current;
                                    var alertEnd = mask.Start;
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

                    if (mergedActuals.Any())
                    {
                        foreach (var act in mergedActuals)
                        {
                            var current = act.Start;
                            var end = act.End;
                            foreach (var valid in validWorkWindows)
                            {
                                if (valid.End <= current) continue;
                                if (valid.Start >= end) break;
                                if (valid.Start > current)
                                {
                                    var alertStart = current;
                                    var alertEnd = valid.Start;
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

        private string GetActivityColor(string? activityName, string? subActivityName, string? awsStatusName)
        {
            string nameToCheck = awsStatusName;

            if (string.IsNullOrEmpty(nameToCheck) || nameToCheck == "-")
            {
                nameToCheck = !string.IsNullOrEmpty(subActivityName) && subActivityName != "-"
                              ? subActivityName
                              : activityName;
            }

            var name = (nameToCheck ?? "").ToLower();

            if (name.Contains("lunch")) return "activity-lunch";
            if (name.Contains("engagement") || name.Contains("corp")) return "activity-corporate";
            if (name.Contains("peer")) return "activity-peer";
            if (name.Contains("meeting")) return "activity-meeting";
            if (name.Contains("training")) return "activity-training";
            if (name.Contains("coaching")) return "activity-coaching";

            if (name.Contains("customer") || name.Contains("phone") ||
                name.Contains("queue") || name.Contains("chat") ||
                name.Contains("email")) return "activity-ol-customer";

            if (name.Contains("admin")) return "activity-admin";
            if (name.Contains("system")) return "activity-system";
            if (name.Contains("available")) return "activity-available";

            if (name.Contains("break")) return "activity-break";
            if (name.Contains("offline")) return "activity-offline";

            return "activity-default";
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