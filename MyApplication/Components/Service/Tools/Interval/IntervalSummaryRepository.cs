using Dapper;
using Microsoft.Data.SqlClient;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service;

public sealed class IntervalSummaryRepository : IIntervalSummaryRepository
{
    private readonly IDbConnectionFactory _factory;
    public IntervalSummaryRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<int> InsertAsync(IntervalSummary m, CancellationToken ct = default)
    {
        await using var conn = _factory.Create("AOM");

        const string sql = @"
INSERT INTO Tools.IntervalSummary
(
  CurrentUser, IntervalDate, IntervalStart, IntervalEnd,

  CurrentUsnASA, CurrentUsnCallsOffered, CurrentUsnCallsAnswered,
  CurrentVipASA, CurrentVipCallsOffered, CurrentVipCallsAnswered,
  CurrentSiprASA, CurrentSiprCallsOffered, CurrentSiprCallsAnswered,
  CurrentNnpiASA, CurrentNnpiCallsOffered, CurrentNnpiCallsAnswered,

  MtdUsnASA, MtdUsnCallsOffered, MtdUsnCallsAnswered,
  MtdVipASA, MtdVipCallsOffered, MtdVipCallsAnswered,
  MtdSiprASA, MtdSiprCallsOffered, MtdSiprCallsAnswered,
  MtdNnpiASA, MtdNnpiCallsOffered, MtdNnpiCallsAnswered,

  Slr33EmMtdLos1, Slr33EmMtdLos2, Slr33VmMtdLos1, Slr33VmMtdLos2,

  CurrentEmailCount, CurrentEmailOldest,
  CurrentCustomerCareCount, CurrentCustomerCareOldest,
  CurrentSiprEmailCount, CurrentSiprEmailOldest,
  CurrentSiprGdaSpreadsheets, CurrentSiprGdaOldest,
  CurrentSiprUaifCount, CurrentSiprUaifOldest,

  CurrentVmCount, CurrentVmOldest,
  CurrentEssCount, CurrentEssOldest,

  BlSrmUaAutoCount, BlSrmUaAutoOldest,
  BlSrmUaUsnManCount, BlSrmUaUsnManOldest,
  BlSrmUaSocManCount, BlSrmUaSocManOldest,

  BlSrmValidationCount, BlSrmValidationOldest,
  BlSrmValidationFailCount, BlSrmValidationFailOldest,
  BlSrmEmailBuildoutsCount, BlSrmEmailBuildoutsOldest,

  BlSrmAfuCount, BlSrmAfuOldest,
  BlSrmCxSatCount, BlSrmCxSatOldest,

  BlOcmNiprReadyCount, BlOcmNiprReadyOldest,
  BlOcmSiprReadyCount, BlOcmSiprReadyOldest,

  BlOcmNiprHoldCount, BlOcmNiprHoldOldest,
  BlOcmSiprHoldCount, BlOcmSiprHoldOldest,

  BlOcmNiprFatalCount, BlOcmNiprFatalOldest,
  BlOcmSiprFatalCount, BlOcmSiprFatalOldest,

  BlRdmUsnCount, BlRdmUsnOldest,
  BlRdmUsnEsdCount, BlRdmUsnEsdOldest,

  NnpiQueue, SiprQueue, NcisQueue, VipQueue, RdmNnpiQueue, RdmSiprQueue,

  NaTodaysFocusArea, NaMajorCirImpact, NaImpactingEvents, NaHpsmStatus, NaManagementNotes
)
VALUES
(
  @CurrentUser, @IntervalDate, @IntervalStart, @IntervalEnd,

  @CurrentUsnASA, @CurrentUsnCallsOffered, @CurrentUsnCallsAnswered,
  @CurrentVipASA, @CurrentVipCallsOffered, @CurrentVipCallsAnswered,
  @CurrentSiprASA, @CurrentSiprCallsOffered, @CurrentSiprCallsAnswered,
  @CurrentNnpiASA, @CurrentNnpiCallsOffered, @CurrentNnpiCallsAnswered,

  @MtdUsnASA, @MtdUsnCallsOffered, @MtdUsnCallsAnswered,
  @MtdVipASA, @MtdVipCallsOffered, @MtdVipCallsAnswered,
  @MtdSiprASA, @MtdSiprCallsOffered, @MtdSiprCallsAnswered,
  @MtdNnpiASA, @MtdNnpiCallsOffered, @MtdNnpiCallsAnswered,

  @Slr33EmMtdLos1, @Slr33EmMtdLos2, @Slr33VmMtdLos1, @Slr33VmMtdLos2,

  @CurrentEmailCount, @CurrentEmailOldest,
  @CurrentCustomerCareCount, @CurrentCustomerCareOldest,
  @CurrentSiprEmailCount, @CurrentSiprEmailOldest,
  @CurrentSiprGdaSpreadsheets, @CurrentSiprGdaOldest,
  @CurrentSiprUaifCount, @CurrentSiprUaifOldest,

  @CurrentVmCount, @CurrentVmOldest,
  @CurrentEssCount, @CurrentEssOldest,

  @BlSrmUaAutoCount, @BlSrmUaAutoOldest,
  @BlSrmUaUsnManCount, @BlSrmUaUsnManOldest,
  @BlSrmUaSocManCount, @BlSrmUaSocManOldest,

  @BlSrmValidationCount, @BlSrmValidationOldest,
  @BlSrmValidationFailCount, @BlSrmValidationFailOldest,
  @BlSrmEmailBuildoutsCount, @BlSrmEmailBuildoutsOldest,

  @BlSrmAfuCount, @BlSrmAfuOldest,
  @BlSrmCxSatCount, @BlSrmCxSatOldest,

  @BlOcmNiprReadyCount, @BlOcmNiprReadyOldest,
  @BlOcmSiprReadyCount, @BlOcmSiprReadyOldest,

  @BlOcmNiprHoldCount, @BlOcmNiprHoldOldest,
  @BlOcmSiprHoldCount, @BlOcmSiprHoldOldest,

  @BlOcmNiprFatalCount, @BlOcmNiprFatalOldest,
  @BlOcmSiprFatalCount, @BlOcmSiprFatalOldest,

  @BlRdmUsnCount, @BlRdmUsnOldest,
  @BlRdmUsnEsdCount, @BlRdmUsnEsdOldest,

  @NnpiQueue, @SiprQueue, @NcisQueue, @VipQueue, @RdmNnpiQueue, @RdmSiprQueue,

  @NaTodaysFocusArea, @NaMajorCirImpact, @NaImpactingEvents, @NaHpsmStatus, @NaManagementNotes
);
SELECT SCOPE_IDENTITY();";

        // Create CommandDefinition to pass cancellation token to Dapper
        var cmd = new CommandDefinition(sql, m, cancellationToken: ct);
        return await conn.ExecuteScalarAsync<int>(cmd);
    }

    public async Task<IntervalSummary?> GetLatestAsync(CancellationToken ct = default)
    {
        await using var conn = _factory.Create("AOM");
        const string sql = "SELECT TOP 1 * FROM Tools.IntervalSummary ORDER BY Id DESC";

        // Create CommandDefinition to pass cancellation token to Dapper
        var cmd = new CommandDefinition(sql, cancellationToken: ct);
        return await conn.QueryFirstOrDefaultAsync<IntervalSummary>(cmd);
    }
}