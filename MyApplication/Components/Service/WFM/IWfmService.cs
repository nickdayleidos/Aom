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
        Task DeleteBreakScheduleAsync(int id);
    }

    // DTO to help display names in the table instead of just IDs
    public class BreakScheduleDto
    {
        public BreakSchedules Schedule { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public string MonTemplate { get; set; } = null!;
        public string TueTemplate { get; set; } = null!;
        public string WedTemplate { get; set; } = null!;
        public string ThuTemplate { get; set; } = null!;
        public string FriTemplate { get; set; } = null!;
        public string SatTemplate { get; set; } = null!;
        public string SunTemplate { get; set; } = null!;
    }
}