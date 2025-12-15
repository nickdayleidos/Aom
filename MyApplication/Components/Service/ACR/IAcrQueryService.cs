using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Service.Acr;

namespace MyApplication.Components.Service.Acr;

public interface IAcrQueryService
{
    // Search & List
    Task<PagedResult<AcrRequestListItem>> SearchAsync(AcrIndexFilter filter, int pageIndex, int pageSize, string? sortLabel, bool sortDescending, CancellationToken ct = default);
    Task<List<AcrRequestListItem>> QueryAsync(AcrIndexFilter filter, int take = 1000, CancellationToken ct = default);

    // Lookups
    Task<List<KeyValuePair<int, string>>> GetOrganizationsAsync(CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetSubOrganizationsAsync(int? orgId, CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetSitesAsync(CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetEmployersAsync(CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default);
    Task<Dictionary<int, string>> GetTypesAsync(CancellationToken ct = default);
    Task<Dictionary<int, string>> GetStatusesAsync(CancellationToken ct = default);
    Task<List<Employees>> GetEmployeesLookupAsync(bool? isActive, CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetOvertimeTypesAsync(CancellationToken ct = default);

    // Details & Edit
    Task<AcrDetailsVm> LoadDetailsAsync(int id, CancellationToken ct = default);
    Task<AcrEditVm> LoadForEditAsync(int id, CancellationToken ct = default);

    // FIX: This now refers to the DTO class, not a nested class
    Task<PrevDetailsVm?> GetPrevDetailsAsync(int employeeId, TimeDisplayMode mode, CancellationToken ct = default);

    // Helpers
    Task<string?> GetStatusNameByAcrIdAsync(int acrRequestId, CancellationToken ct = default);
    Task<int?> GetStatusIdByAcrIdAsync(int acrId, CancellationToken ct = default);
    Task<string?> GetSiteTimeZoneAsync(int siteId, CancellationToken ct = default);
    Task<Employees?> GetEmployeeAsync(int employeeId, CancellationToken ct = default);
    Task<EmployeeHistorySnapshot?> GetLatestEmployeeHistoryAsync(int employeeId, CancellationToken ct = default);
}
