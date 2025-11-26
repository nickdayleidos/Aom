using MyApplication.Components.Model.AOM.Employee;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyApplication.Common.Time;

namespace MyApplication.Components.Service.Acr;

public interface IAcrQueryService
{
    // ... existing lookups ...
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

    // ... existing details ...
    Task<AcrDetailsVm> LoadDetailsAsync(int id, CancellationToken ct = default);
    Task<AcrEditVm> LoadForEditAsync(int id, CancellationToken ct = default);
    Task<AcrQueryService.PrevDetailsVm?> GetPrevDetailsAsync(int employeeId, TimeDisplayMode mode, CancellationToken ct = default);
    Task<EmployeeHistorySnapshot?> GetLatestEmployeeHistoryAsync(int employeeId, CancellationToken ct = default);
    Task<string?> GetStatusNameByAcrIdAsync(int acrRequestId, CancellationToken ct = default);
    Task<int?> GetStatusIdByAcrIdAsync(int acrId, CancellationToken ct = default);
    Task<Employees?> GetEmployeeAsync(int employeeId, CancellationToken ct = default);
    Task<string?> GetSiteTimeZoneAsync(int siteId, CancellationToken ct = default);

    // Legacy Query (keep if needed, or remove if unused)
    Task<List<AcrRequestListItem>> QueryAsync(AcrIndexFilter filter, int take = 1000, CancellationToken ct = default);

    // NEW: Server-side Search
    Task<PagedResult<AcrRequestListItem>> SearchAsync(
        AcrIndexFilter filter,
        int pageIndex,
        int pageSize,
        string? sortLabel,
        bool sortDescending,
        CancellationToken ct = default);
}