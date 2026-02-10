using MyApplication.Components.Model.AOM.Employee;
using EmployeeEntity = MyApplication.Components.Model.AOM.Employee.Employees;

namespace MyApplication.Components.Service
{
    public record OperaSubTypeDto(int Id, string Name, int ActivityTypeId);

    public sealed class OperaQuery
    {
        public string? NameOrId { get; set; }

        // Legacy single filters (kept for backward compatibility)
        public int? StatusId { get; set; }
        public int? ManagerId { get; set; }
        public int? SupervisorId { get; set; }
        public int? ActivityTypeId { get; set; }
        public int? ActivitySubTypeId { get; set; }

        // New multi-select filters
        public IReadOnlyCollection<int>? StatusIds { get; set; }
        public IReadOnlyCollection<int>? ManagerIds { get; set; }
        public IReadOnlyCollection<int>? SupervisorIds { get; set; }
        public IReadOnlyCollection<int>? ActivityTypeIds { get; set; }
        public IReadOnlyCollection<int>? ActivitySubTypeIds { get; set; }
        public IReadOnlyCollection<string>? SubmittedBys { get; set; }

        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
        public int Take { get; set; } = 1000;
    }

    public interface IOperaRepository
    {
        Task<(IReadOnlyList<OperaRequest> Items, int Total)> SearchAsync(OperaQuery q, CancellationToken ct = default);
        Task<OperaRequest?> GetAsync(int requestId, CancellationToken ct = default);
        Task<int> CreateManyAsync(IEnumerable<OperaRequest> requests, CancellationToken ct = default);
        Task UpdateAsync(OperaRequest req, CancellationToken ct = default);
        Task SetStatusAsync(int requestId, int statusId, string actor, CancellationToken ct = default);

        Task<IReadOnlyList<OperaSubTypeDto>> GetSubTypesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<OperaStatus>> GetStatusesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<EmployeeEntity>> SearchEmployeesAsync(string term, int take = 20, CancellationToken ct = default);

        Task<Dictionary<int, string>> GetActivityTypesAsync(CancellationToken ct = default);
        Task<Dictionary<int, string>> GetActivitySubTypesAsync(CancellationToken ct = default);
        Task<Dictionary<int, string?>> GetEmployeeSiteTimeZonesAsync(IEnumerable<int> employeeIds, CancellationToken ct = default);
        Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default);
        Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default);

        Task<IReadOnlyList<OperaTimeframe>> GetTimeframesAsync(CancellationToken ct = default);

        Task<DateTime?> GetEndOfShiftAsync(int employeeId, DateTime startTime, CancellationToken ct = default);
        Task<DateTime?> GetStartOfShiftAsync(int employeeId, DateTime dateEt, CancellationToken ct = default);

        Task SaveOperaRequestAsync(OperaRequest request, CancellationToken ct = default);
        Task<OperaRequest> GetRequestByIdAsync(int id, CancellationToken ct = default);
        Task UpdateRequestAsync(OperaRequest request, CancellationToken ct = default);

        Task<List<DetailedSchedule>> GetEmployeeWeeklyScheduleAsync(int employeeId, DateOnly weekStart, CancellationToken ct = default);

        // NEW: SubmittedBy lookup for /opera filter dropdown
        Task<IReadOnlyList<string>> GetOperaSubmittersAsync(CancellationToken ct = default);
    }
}
