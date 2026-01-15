using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MyApplication.Components.Model.AOM;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Services.Email;

namespace MyApplication.Components.Service;

public sealed class IntervalEmailService
{
    private readonly IIntervalSummaryRepository _repo;
    private readonly AuthenticationStateProvider _auth;
    private readonly ISnackbar _snackbar;
    private readonly IEmailComposer _composer;

    public IntervalEmailService(
        IIntervalSummaryRepository repo,
        AuthenticationStateProvider auth,
        ISnackbar snackbar,
        IEmailComposer composer)
    {
        _repo = repo;
        _auth = auth;
        _snackbar = snackbar;
        _composer = composer;
    }

    public async Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
        ComposeAndLogAsync(IntervalEmailContext ctx, CancellationToken ct = default)
    {
        // 1) Compose email from current template
        var (subject, bodyHtml, to, cc, from) = await _composer.ComposeAsync("IntervalSummary", ctx, ct);

        // 2) Persist a new IntervalSummary row
        var authState = await _auth.GetAuthenticationStateAsync();
        var user = authState.User.Identity?.Name ?? "Unknown";

        // Helpers to convert double -> decimal?
        static decimal? D(double val) => val == 0 ? null : (decimal)val;

        var row = new IntervalSummary
        {
            CurrentUser = user,
            IntervalDate = ctx.DateLocal,
            IntervalStart = ctx.IntervalStart,
            IntervalEnd = ctx.IntervalEnd,

            // Current
            CurrentUsnASA = (int)ctx.UsnAsa,
            CurrentUsnCallsOffered = ctx.UsnCallsOffered,
            CurrentUsnCallsAnswered = ctx.UsnCallsAnswered,
            CurrentVipASA = (int)ctx.VipAsa,
            CurrentVipCallsOffered = ctx.VipCallsOffered,
            CurrentVipCallsAnswered = ctx.VipCallsAnswered,
            CurrentSiprASA = (int)ctx.SiprAsa,
            CurrentSiprCallsOffered = ctx.SiprCallsOffered,
            CurrentSiprCallsAnswered = ctx.SiprCallsAnswered,
            CurrentNnpiASA = (int)ctx.NnpiAsa,
            CurrentNnpiCallsOffered = ctx.NnpiCallsOffered,
            CurrentNnpiCallsAnswered = ctx.NnpiCallsAnswered,

            // MTD
            MtdUsnASA = (int)ctx.MtdUsnAsa,
            MtdUsnCallsOffered = ctx.MtdUsnCallsOffered,
            MtdUsnCallsAnswered = ctx.MtdUsnCallsAnswered,
            MtdVipASA = (int)ctx.MtdVipAsa,
            MtdVipCallsOffered = ctx.MtdVipCallsOffered,
            MtdVipCallsAnswered = ctx.MtdVipCallsAnswered,
            MtdSiprASA = (int)ctx.MtdSiprAsa,
            MtdSiprCallsOffered = ctx.MtdSiprCallsOffered,
            MtdSiprCallsAnswered = ctx.MtdSiprCallsAnswered,
            MtdNnpiASA = (int)ctx.MtdNnpiAsa,
            MtdNnpiCallsOffered = ctx.MtdNnpiCallsOffered,
            MtdNnpiCallsAnswered = ctx.MtdNnpiCallsAnswered,

            // SLR
            Slr33EmMtdLos1 = D(ctx.Slr33EmMtdLos1),
            Slr33EmMtdLos2 = D(ctx.Slr33EmMtdLos2),
            Slr33VmMtdLos1 = D(ctx.Slr33VmMtdLos1),
            Slr33VmMtdLos2 = D(ctx.Slr33VmMtdLos2),

            // Backlog - Wireless
            CurrentEmailCount = ctx.CurrentEmailCount,
            CurrentEmailOldest = D(ctx.CurrentEmailOldest),
            CurrentCustomerCareCount = ctx.CurrentCustomerCareCount,
            CurrentCustomerCareOldest = D(ctx.CurrentCustomerCareOldest),
            CurrentSiprEmailCount = ctx.CurrentSiprEmailCount,
            CurrentSiprEmailOldest = D(ctx.CurrentSiprEmailOldest),
            CurrentSiprGdaSpreadsheets = ctx.CurrentSiprGdaSpreadsheets,
            CurrentSiprGdaOldest = D(ctx.CurrentSiprGdaOldest),
            CurrentSiprUaifCount = ctx.CurrentSiprUaifCount,
            CurrentSiprUaifOldest = D(ctx.CurrentSiprUaifOldest),

            // Backlog - Wireline
            CurrentVmCount = ctx.CurrentVmCount,
            CurrentVmOldest = D(ctx.CurrentVmOldest),
            CurrentEssCount = ctx.CurrentEssCount,
            CurrentEssOldest = D(ctx.CurrentEssOldest),

            // SRM
            BlSrmUaAutoCount = ctx.SrmUaAutoCount,
            BlSrmUaAutoOldest = D(ctx.SrmUaAutoAgeHours),
            BlSrmUaUsnManCount = ctx.SrmUaUsnManCount,
            BlSrmUaUsnManOldest = D(ctx.SrmUaUsnManAgeHours),
            BlSrmUaSocManCount = ctx.SrmUaSocManCount,
            BlSrmUaSocManOldest = D(ctx.SrmUaSocManAgeHours),

            BlSrmValidationCount = ctx.SrmValCount,
            BlSrmValidationOldest = D(ctx.SrmValAgeDays),
            BlSrmValidationFailCount = ctx.SrmValLineFailCount,
            BlSrmValidationFailOldest = D(ctx.SrmValLineFailAgeDays),
            BlSrmEmailBuildoutsCount = ctx.SrmValEmailCount,
            BlSrmEmailBuildoutsOldest = D(ctx.SrmValEmailAgeDays),

            BlSrmAfuCount = ctx.AfuCount,
            BlSrmAfuOldest = D(ctx.AfuAgeHours),
            BlSrmCxSatCount = ctx.CsCount,
            BlSrmCxSatOldest = D(ctx.CsAgeHours),

            // OCM
            BlOcmNiprReadyCount = ctx.OcmNiprReadyCount,
            BlOcmNiprReadyOldest = D(ctx.OcmNiprReadyAgeHours),
            BlOcmSiprReadyCount = ctx.OcmSiprReadyCount,
            BlOcmSiprReadyOldest = D(ctx.OcmSiprReadyAgeHours),

            BlOcmNiprHoldCount = ctx.OcmNiprHoldCount,
            BlOcmNiprHoldOldest = D(ctx.OcmNiprHoldAgeHours),
            BlOcmSiprHoldCount = ctx.OcmSiprHoldCount,
            BlOcmSiprHoldOldest = D(ctx.OcmSiprHoldAgeHours),

            BlOcmNiprFatalCount = ctx.OcmNiprFatalCount,
            BlOcmNiprFatalOldest = D(ctx.OcmNiprFatalAgeHours),
            BlOcmSiprFatalCount = ctx.OcmSiprFatalCount,
            BlOcmSiprFatalOldest = D(ctx.OcmSiprFatalAgeHours),

            // RDM
            BlRdmUsnCount = ctx.RdmUsnCount,
            BlRdmUsnOldest = D(ctx.RdmUsnAgeDays),
            BlRdmUsnEsdCount = ctx.RdmEsdUsnCount,
            BlRdmUsnEsdOldest = D(ctx.RdmEsdUsnAgeDays),

            // Other Backlog Queues
            NnpiQueue = ctx.NnpiQueue,
            SiprQueue = ctx.SiprQueue,
            NcisQueue = ctx.NcisQueue,
            VipQueue = ctx.VipQueue,
            RdmNnpiQueue = ctx.RdmNnpiQueue,
            RdmSiprQueue = ctx.RdmSiprQueue,

            // Notes - UPDATED
            NaTodaysFocusArea = ctx.NaTodaysFocusArea,
            NaMajorCirImpact = ctx.NaMajorCirImpact,
            NaImpactingEvents = ctx.NaImpactingEvents,
            NaHpsmStatus = ctx.NaHpsmStatus,
            NaManagementNotes = ctx.NaManagementNotes
        };

        try
        {
            await _repo.InsertAsync(row, ct);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Failed to log interval to DB: {ex.Message}", Severity.Error);
            throw;
        }

        return (subject, bodyHtml, to, cc, from);
    }
}