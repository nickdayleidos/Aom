using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Employee;

public sealed class EmployeeDetailsService
{
    private readonly IDbContextFactory<AomDbContext> _contextFactory;

    public EmployeeDetailsService(IDbContextFactory<AomDbContext> contextFactory)
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

    public async Task<EmployeeDetailDto?> GetDetailAsync(int employeeId, CancellationToken ct = default)
    {
        return null;
    }
}
