using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Wfm
{
    public class WfmService : IWfmService
    {
        // Change from AomDbContext to IDbContextFactory
        private readonly IDbContextFactory<AomDbContext> _factory;

        public WfmService(IDbContextFactory<AomDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<BreakTemplates>> GetBreakTemplatesAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.BreakTemplates
                .AsNoTracking() // Important: Don't track read-only lists
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task SaveBreakTemplateAsync(BreakTemplates template)
        {
            using var context = await _factory.CreateDbContextAsync();

            if (template.Id == 0)
            {
                context.BreakTemplates.Add(template);
            }
            else
            {
                // Because this is a fresh context, there is no existing tracking conflict
                context.BreakTemplates.Update(template);
            }
            await context.SaveChangesAsync();
        }

        public async Task<List<Employees>> GetEmployeesAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Employees
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.LastName)
                .ToListAsync();
        }

        public async Task<List<BreakScheduleDto>> GetBreakSchedulesAsync()
        {
            using var context = await _factory.CreateDbContextAsync();

            // Fetch raw data
            var schedules = await context.BreakSchedules.AsNoTracking().ToListAsync();
            var employees = await context.Employees.AsNoTracking().ToDictionaryAsync(k => k.Id, v => $"{v.LastName}, {v.FirstName}");
            var templates = await context.BreakTemplates.AsNoTracking().ToDictionaryAsync(k => k.Id, v => v.Name);

            // Map to DTO for display
            var result = new List<BreakScheduleDto>();
            foreach (var s in schedules)
            {
                result.Add(new BreakScheduleDto
                {
                    Schedule = s,
                    EmployeeName = employees.ContainsKey(s.EmployeeId) ? employees[s.EmployeeId] : $"Unknown ({s.EmployeeId})",
                    MonTemplate = s.MondayTemplateId.HasValue && templates.ContainsKey(s.MondayTemplateId.Value) ? templates[s.MondayTemplateId.Value] : "-",
                    TueTemplate = s.TuesdayTemplateId.HasValue && templates.ContainsKey(s.TuesdayTemplateId.Value) ? templates[s.TuesdayTemplateId.Value] : "-",
                    WedTemplate = s.WednesdayTemplateId.HasValue && templates.ContainsKey(s.WednesdayTemplateId.Value) ? templates[s.WednesdayTemplateId.Value] : "-",
                    ThuTemplate = s.ThursdayTemplateId.HasValue && templates.ContainsKey(s.ThursdayTemplateId.Value) ? templates[s.ThursdayTemplateId.Value] : "-",
                    FriTemplate = s.FridayTemplateId.HasValue && templates.ContainsKey(s.FridayTemplateId.Value) ? templates[s.FridayTemplateId.Value] : "-",
                    SatTemplate = s.SaturdayTemplateId.HasValue && templates.ContainsKey(s.SaturdayTemplateId.Value) ? templates[s.SaturdayTemplateId.Value] : "-",
                    SunTemplate = s.SundayTemplateID.HasValue && templates.ContainsKey(s.SundayTemplateID.Value) ? templates[s.SundayTemplateID.Value] : "-"
                });
            }
            return result;
        }

        public async Task SaveBreakScheduleAsync(BreakSchedules schedule)
        {
            using var context = await _factory.CreateDbContextAsync();

            if (schedule.Id == 0)
                context.BreakSchedules.Add(schedule);
            else
                context.BreakSchedules.Update(schedule);

            await context.SaveChangesAsync();
        }
    }
}