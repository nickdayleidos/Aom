using System.Threading;
using System.Threading.Tasks;

namespace MyApplication.Components.Service.Acr
{
    public interface IAcrService
    {
        /// <summary>Create a new ACR and any applicable child rows (Org, Schedule, OvertimeSchedules).</summary>
        Task<int> CreateAsync(AcrCreateVm vm, CancellationToken ct = default);

        /// <summary>Update an existing ACR and any applicable child rows (Org, Schedule, OvertimeSchedules).</summary>
        Task UpdateAsync(AcrEditVm vm, CancellationToken ct = default);

        /// <summary>
        /// Return the last ACR for this employee that contains any overtime adjustment in AcrOvertimeSchedules.
        /// Returns null if none exists.
        /// </summary>
        Task<LastOvertimeAdjustment?> GetLastOvertimeAdjustmentAsync(int employeeId, CancellationToken ct = default);
        Task SetStatusAsync(int acrRequestId, int newStatusId, CancellationToken ct = default);
    }

    public sealed record LastOvertimeAdjustment(
        int AcrRequestId,
        AcrTypeKey TypeKey,
        DateOnly EffectiveDate,
        OvertimeAdjustmentDto Overtime
    );
}
