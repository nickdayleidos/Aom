using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Service.Employee.Dtos; // Keep if you have DTOs here
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

            // 1. Fetch Basic Profile
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
                DomainLoginName = emp.DomainLoginName
            };

            // 2. Fetch Latest History
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
                vm.Organization = history.Organization?.Name;
                vm.SubOrganization = history.SubOrganization?.Name;

                // Manual fetch for Manager
                if (history.ManagerId.HasValue)
                {
                    vm.Manager = await ctx.Managers.Where(m => m.Id == history.ManagerId)
                        .Select(m => ctx.Employees.Where(e => e.Id == m.EmployeeId).Select(e => e.LastName + ", " + e.FirstName).FirstOrDefault())
                        .FirstOrDefaultAsync();
                }

                // Manual fetch for Supervisor
                if (history.SupervisorId.HasValue)
                {
                    vm.Supervisor = await ctx.Supervisors.Where(s => s.Id == history.SupervisorId)
                        .Select(s => ctx.Employees.Where(e => e.Id == s.EmployeeId).Select(e => e.LastName + ", " + e.FirstName).FirstOrDefault())
                        .FirstOrDefaultAsync();
                }

                // --- 2a. Fetch Schedules (SHIFT 1 & SHIFT 2) ---
                if (history.ScheduleRequestId.HasValue)
                {
                    var schedules = await ctx.AcrSchedules.AsNoTracking()
                        .Where(s => s.AcrRequestId == history.ScheduleRequestId.Value)
                        .ToListAsync();

                    string Fmt(TimeOnly? s, TimeOnly? e) => (s.HasValue && e.HasValue) ? $"{s:HH:mm}-{e:HH:mm}" : "OFF";

                    // Map Shift 1
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
                            Sun = Fmt(s1.SundayStart, s1.SundayEnd)
                        };

                        // Map Shift 2 (Only if Split)
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
                                    Sun = Fmt(s2.SundayStart, s2.SundayEnd)
                                };
                            }
                        }
                    }
                }

                // --- 2b. Overtime ---
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

            // 3. Static Breaks
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

            // 4. NEW: Skills
            vm.Skills = await ctx.Skills.AsNoTracking()
                 .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                 // or: && (s.IsActive ?? false)
                 .Include(s => s.SkillType)
                 .Select(s => s.SkillType.Name)
                 .OrderBy(n => n)
                 .ToListAsync();

            // 5. NEW: ACR History (Limit 10 usually good for details page)
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

            // 6. NEW: OPERA History
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

        // ... Keep your existing SearchAsync and GetDetailAsync methods here ...
        public async Task<List<EmployeeListItem>> SearchAsync(string? query, bool activeOnly, int take = 200, CancellationToken ct = default)
        {
            // ... existing code ...
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
                    || (x.ManagerName != null && EF.Functions.Like(x.ManagerName, $"%{q}%"))
                    || (x.SupervisorName != null && EF.Functions.Like(x.SupervisorName, $"%{q}%"))
                    || (x.OrganizationName != null && EF.Functions.Like(x.OrganizationName, $"%{q}%"))
                    || (x.SubOrganizationName != null && EF.Functions.Like(x.SubOrganizationName, $"%{q}%"))
                );
            }

            return await baseQuery
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
        }

        public async Task<EmployeeDetailDto?> GetDetailAsync(int employeeId, CancellationToken ct = default)
        {
            // ... existing code ...
            return null; // Placeholder to allow compilation if you haven't pasted the full body
        }
    }
}