using Dapper;
using Microsoft.Data.SqlClient;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service;

public sealed class IntervalSummaryRepository : IIntervalSummaryRepository
{
    private readonly IDbConnectionFactory _factory;
    public IntervalSummaryRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<int> InsertAsync(IntervalSummary row, CancellationToken ct = default)
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

  NaTodaysFocusArea, NaMajorCirImpact, NaImpactingEvents, NaHpsmStatus, NaManagementNotes
)
OUTPUT INSERTED.Id
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

  @NaTodaysFocusArea, @NaMajorCirImpact, @NaImpactingEvents, @NaHpsmStatus, @NaManagementNotes
);";

        var newId = await conn.ExecuteScalarAsync<int>(sql, row);
        return newId;
    }
}
