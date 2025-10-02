using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service
{
    public sealed class OperaQuery
    {
        public string? NameOrId { get; set; }    // matches first/last or numeric ids
        public int? StatusId { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
        public int Take { get; set; } = 1000;    // hard limit
    }

    public interface IOperaRepository
    {
        Task<(IReadOnlyList<OperaRequest> Items, int Total)> SearchAsync(OperaQuery q, CancellationToken ct = default);
        Task<OperaRequest?> GetAsync(int requestId, CancellationToken ct = default);
        Task<int> CreateManyAsync(IEnumerable<OperaRequest> requests, CancellationToken ct = default);
        Task UpdateAsync(OperaRequest req, CancellationToken ct = default);
        Task SetStatusAsync(int requestId, int statusId, string actor, CancellationToken ct = default);
        Task<IReadOnlyList<OperaStatus>> GetStatusesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Employees>> SearchEmployeesAsync(string term, int take = 20, CancellationToken ct = default);
    }
}
