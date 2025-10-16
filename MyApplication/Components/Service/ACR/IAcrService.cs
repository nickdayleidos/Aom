using System.Threading;
using System.Threading.Tasks;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Acr
{
    public interface IAcrService
    {
        Task<int> CreateOrganizationChangeAsync(OrganizationChangeDto dto, CancellationToken ct = default);
        Task<int> CreateScheduleChangeAsync(ScheduleChangeDto dto, CancellationToken ct = default);
        Task<int> CreateOrgScheduleAsync(OrganizationChangeDto org, ScheduleChangeDto sch, CancellationToken ct = default);
        Task<int> CreateNewHireAsync(NewHireDto dto, CancellationToken ct = default);
        Task<int> CreateSeparationAsync(SeparationDto dto, CancellationToken ct = default);
        Task<int> CreateRehireAsync(RehireDto dto); // returns new ACR Id

        // Details / workflow
        Task<AcrRequest?> GetAsync(int id, CancellationToken ct = default);
        Task UpdateStatusAsync(int id, int newStatusId, CancellationToken ct = default);
    }
}
