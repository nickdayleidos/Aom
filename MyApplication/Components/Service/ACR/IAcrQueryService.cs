using MyApplication.Common.Time;
using MyApplication.Components.Model.AOM.Employee;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static MyApplication.Components.Service.Acr.AcrQueryService;

namespace MyApplication.Components.Service.Acr;

public interface IAcrQueryService
{
    Task<List<AcrRequestListItem>> QueryAsync(AcrIndexFilter filter, int take = 1000, CancellationToken ct = default);

    Task<Dictionary<int, string>> GetTypesAsync(CancellationToken ct = default);
    Task<Dictionary<int, string>> GetStatusesAsync(CancellationToken ct = default);

    Task<List<Employees>> GetEmployeesLookupAsync(bool? isActive, CancellationToken ct = default);
    Task<EmployeeHistorySnapshot?> GetLatestEmployeeHistoryAsync(int employeeId, CancellationToken ct = default);

    Task<AcrEditVm> LoadForEditAsync(int acrRequestId, CancellationToken ct = default);
    Task<AcrDetailsVm> LoadDetailsAsync(int acrRequestId, CancellationToken ct = default);

    // Lookups used by forms (aligned to your model)
    Task<List<KeyValuePair<int, string>>> GetOrganizationsAsync(CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetSubOrganizationsAsync(int? orgId, CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetSitesAsync(CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetEmployersAsync(CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default);
    Task<string?> GetStatusNameByAcrIdAsync(int acrRequestId, CancellationToken ct = default);
    Task<PrevDetailsVm?> GetPrevDetailsAsync(int employeeId, TimeDisplayMode mode, CancellationToken ct = default);
    Task<List<KeyValuePair<int, string>>> GetOvertimeTypesAsync(CancellationToken ct = default);   // AcrOvertimeTypes
    Task<string?> GetSiteTimeZoneAsync(int siteId, CancellationToken ct = default);
    Task<Employees?> GetEmployeeAsync(int employeeId, CancellationToken ct = default);
    Task<int?> GetStatusIdByAcrIdAsync(int acrId, CancellationToken ct = default);

}
