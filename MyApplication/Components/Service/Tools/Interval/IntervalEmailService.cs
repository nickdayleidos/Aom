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

    /// <summary>
    /// Compose the email (from template) AND persist a new IntervalSummary row
    /// using the same IntervalEmailContext the page shows to the user.
    /// Returns the composed pieces for the .eml download.
    /// </summary>
    public async Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
        ComposeAndLogAsync(IntervalEmailContext ctx, CancellationToken ct = default)
    {
        // 1) Compose email from current template
        var (subject, bodyHtml, to, cc, from) = await _composer.ComposeAsync("IntervalSummary", ctx, ct);

        // 2) Persist a new IntervalSummary row
        var authState = await _auth.GetAuthenticationStateAsync();
        var userName = authState.User?.Identity?.Name ?? "unknown";

        var m = MapToModel(ctx, userName);
        var newId = await _repo.InsertAsync(m, ct);

        _snackbar.Add($"Interval entry saved (ID {newId})", Severity.Success);
        return (subject, bodyHtml, to, cc, from);
    }

    // Components/Service/IntervalEmailService.cs (inside MapToModel)
    private static IntervalSummary MapToModel(IntervalEmailContext ctx, string user)
    {
        static int R(double v) => (int)Math.Round((decimal)v);
        static decimal? D(double v) => decimal.Round((decimal)v, 2);

        return new IntervalSummary
        {
            CurrentUser = user,
            IntervalDate = ctx.DateLocal.Date,
            IntervalStart = ctx.IntervalStart,
            IntervalEnd = ctx.IntervalEnd,

            // Current day
            CurrentUsnASA = R(ctx.UsnAsa),
            CurrentUsnCallsOffered = ctx.UsnCallsOffered,
            CurrentUsnCallsAnswered = ctx.UsnCallsAnswered,

            CurrentVipASA = R(ctx.VipAsa),
            CurrentVipCallsOffered = ctx.VipCallsOffered,
            CurrentVipCallsAnswered = ctx.VipCallsAnswered,

            CurrentSiprASA = R(ctx.SiprAsa),
            CurrentSiprCallsOffered = ctx.SiprCallsOffered,
            CurrentSiprCallsAnswered = ctx.SiprCallsAnswered,

            // If your DB uses CurrentNNPIAsa/CurrentNNPI* keep those names in the entity:
            CurrentNnpiASA = R(ctx.NnpiAsa),
            CurrentNnpiCallsOffered = ctx.NnpiCallsOffered,
            CurrentNnpiCallsAnswered = ctx.NnpiCallsAnswered,

            // MTD
            MtdUsnASA = R(ctx.MtdUsnAsa),
            MtdUsnCallsOffered = ctx.MtdUsnCallsOffered,
            MtdUsnCallsAnswered = ctx.MtdUsnCallsAnswered,

            MtdVipASA = R(ctx.MtdVipAsa),
            MtdVipCallsOffered = ctx.MtdVipCallsOffered,
            MtdVipCallsAnswered = ctx.MtdVipCallsAnswered,

            MtdSiprASA = R(ctx.MtdSiprAsa),
            MtdSiprCallsOffered = ctx.MtdSiprCallsOffered,
            MtdSiprCallsAnswered = ctx.MtdSiprCallsAnswered,

            MtdNnpiASA = R(ctx.MtdNnpiAsa),
            MtdNnpiCallsOffered = ctx.MtdNnpiCallsOffered,
            MtdNnpiCallsAnswered = ctx.MtdNnpiCallsAnswered,

            // SLR33
            Slr33EmMtdLos1 = R(ctx.Slr33EmLos1),
            Slr33EmMtdLos2 = R(ctx.Slr33EmLos2),
            Slr33VmMtdLos1 = R(ctx.Slr33VmLos1),
            Slr33VmMtdLos2 = R(ctx.Slr33VmLos2),

            // Email / Cust Care / SIPR Email / GDA / UAIF
            CurrentEmailCount = ctx.EmailCount,
            CurrentEmailOldest = D(ctx.EmailOldestHours),
            CurrentCustomerCareCount = ctx.CustCareCount,
            CurrentCustomerCareOldest = D(ctx.CustCareOldestHours),

            CurrentSiprEmailCount = ctx.SiprEmailCount,
            CurrentSiprEmailOldest = D(ctx.SiprEmailOldestHours),
            CurrentSiprGdaSpreadsheets = ctx.SiprGdaCount,
            CurrentSiprGdaOldest = D(ctx.SiprGdaOldestHours),
            CurrentSiprUaifCount = ctx.SiprUaifCount,
            CurrentSiprUaifOldest = D(ctx.SiprUaifOldestDays),

            // VM / ESS
            CurrentVmCount = ctx.VmCount,
            CurrentVmOldest = D(ctx.VmOldestHours),
            CurrentEssCount = ctx.EssCount,
            CurrentEssOldest = D(ctx.EssOldestHours),

            // Backlog – SRM User Admin / Validation / AFU / Incidents
            BlSrmUaAutoCount = ctx.SrmAutoCount,
            BlSrmUaAutoOldest = D(ctx.SrmAutoAgeHours),
            BlSrmUaUsnManCount = ctx.SrmUsnManCount,
            BlSrmUaUsnManOldest = D(ctx.SrmUsnManAgeHours),
            BlSrmUaSocManCount = ctx.SrmSocManCount,
            BlSrmUaSocManOldest = D(ctx.SrmSocManAgeHours),

            BlSrmValidationCount = ctx.SrmValLineCount,
            BlSrmValidationOldest = D(ctx.SrmValLineAgeDays),
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

            // Notes
            NaTodaysFocusArea = ctx.FocusArea,
            NaMajorCirImpact = ctx.CirImpactAsa,
            NaImpactingEvents = ctx.ImpactEvents,
            NaHpsmStatus = ctx.HpsmStatus,
            NaManagementNotes = ctx.ManagementNotes
        };
    }

}
