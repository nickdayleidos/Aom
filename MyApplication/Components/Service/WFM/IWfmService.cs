using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Wfm
{
    public interface IWfmService
    {
        // Templates
        Task<List<BreakTemplates>> GetBreakTemplatesAsync();
        Task SaveBreakTemplateAsync(BreakTemplates template);

        // Static Schedules
        Task<List<BreakScheduleDto>> GetBreakSchedulesAsync();
        Task SaveBreakScheduleAsync(BreakSchedules schedule);

        // Helpers for Dropdowns
        Task<List<Employees>> GetEmployeesAsync();
    }

    // DTO to help display names in the table instead of just IDs
    public class BreakScheduleDto
    {
        public BreakSchedules Schedule { get; set; }
        public string EmployeeName { get; set; }
        public string MonTemplate { get; set; }
        public string TueTemplate { get; set; }
        public string WedTemplate { get; set; }
        public string ThuTemplate { get; set; }
        public string FriTemplate { get; set; }
        public string SatTemplate { get; set; }
        public string SunTemplate { get; set; }
    }
}