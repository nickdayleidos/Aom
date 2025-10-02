// MyApplication/Components/Services/Email/IntervalEmailBuilder.cs
using System.Globalization;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Services.Email;
using static MyApplication.Components.Services.Email.TemplateRenderer;
using AomModel = MyApplication.Components.Model.AOM;

namespace MyApplication.Components.Services.Email;

public static class IntervalEmailBuilder
{
    public static (string Subject, string BodyHtml) Build(
        EmailTemplates tpl,
        IntervalEmailContext ctx,
        CultureInfo? culture = null)
    {
        culture ??= CultureInfo.GetCultureInfo("en-US");

        var tokens = new Dictionary<string, object?>
        {
            ["Date"] = ctx.DateLocal,
            ["Interval"] = ctx.IntervalLabel,
            ["IntervalStart"] = DateTime.Today.Add(ctx.IntervalStart),
            ["IntervalEnd"] = DateTime.Today.Add(ctx.IntervalEnd),

            // Current day
            ["UsnCallsOffered"] = ctx.UsnCallsOffered,
            ["UsnCallsAnswered"] = ctx.UsnCallsAnswered,
            ["UsnAsa"] = ctx.UsnAsa,
            ["VipCallsOffered"] = ctx.VipCallsOffered,
            ["VipCallsAnswered"] = ctx.VipCallsAnswered,
            ["VipAsa"] = ctx.VipAsa,
            ["SiprCallsOffered"] = ctx.SiprCallsOffered,
            ["SiprCallsAnswered"] = ctx.SiprCallsAnswered,
            ["SiprAsa"] = ctx.SiprAsa,
            ["NnpiCallsOffered"] = ctx.NnpiCallsOffered,
            ["NnpiCallsAnswered"] = ctx.NnpiCallsAnswered,
            ["NnpiAsa"] = ctx.NnpiAsa,

            // MTD
            ["MtdUsnCallsOffered"] = ctx.MtdUsnCallsOffered,
            ["MtdUsnCallsAnswered"] = ctx.MtdUsnCallsAnswered,
            ["MtdUsnAsa"] = ctx.MtdUsnAsa,
            ["MtdVipCallsOffered"] = ctx.MtdVipCallsOffered,
            ["MtdVipCallsAnswered"] = ctx.MtdVipCallsAnswered,
            ["MtdVipAsa"] = ctx.MtdVipAsa,
            ["MtdSiprCallsOffered"] = ctx.MtdSiprCallsOffered,
            ["MtdSiprCallsAnswered"] = ctx.MtdSiprCallsAnswered,
            ["MtdSiprAsa"] = ctx.MtdSiprAsa,
            ["MtdNnpiCallsOffered"] = ctx.MtdNnpiCallsOffered,
            ["MtdNnpiCallsAnswered"] = ctx.MtdNnpiCallsAnswered,
            ["MtdNnpiAsa"] = ctx.MtdNnpiAsa,

            // Email / CustCare / SIPR Email
            ["Slr33EmLos1"] = ctx.Slr33EmLos1,
            ["Slr33EmLos2"] = ctx.Slr33EmLos2,
            ["EmailCount"] = ctx.EmailCount,
            ["EmailOldestHours"] = ctx.EmailOldestHours,
            ["CustCareCount"] = ctx.CustCareCount,
            ["CustCareOldestHours"] = ctx.CustCareOldestHours,
            ["SiprEmailCount"] = ctx.SiprEmailCount,
            ["SiprEmailOldestHours"] = ctx.SiprEmailOldestHours,
            ["SiprGdaCount"] = ctx.SiprGdaCount,
            ["SiprGdaOldestHours"] = ctx.SiprGdaOldestHours,
            ["SiprUaifCount"] = ctx.SiprUaifCount,
            ["SiprUaifOldestDays"] = ctx.SiprUaifOldestDays,

            // Voicemail / ESS
            ["Slr33VmLos1"] = ctx.Slr33VmLos1,
            ["Slr33VmLos2"] = ctx.Slr33VmLos2,
            ["VmCount"] = ctx.VmCount,
            ["VmOldestHours"] = ctx.VmOldestHours,
            ["EssCount"] = ctx.EssCount,
            ["EssOldestHours"] = ctx.EssOldestHours,

            // Backlog
            ["SrmAutoCount"] = ctx.SrmAutoCount,
            ["SrmAutoAgeHours"] = ctx.SrmAutoAgeHours,
            ["SrmUsnManCount"] = ctx.SrmUsnManCount,
            ["SrmUsnManAgeHours"] = ctx.SrmUsnManAgeHours,
            ["SrmSocManCount"] = ctx.SrmSocManCount,
            ["SrmSocManAgeHours"] = ctx.SrmSocManAgeHours,
            ["SrmValLineCount"] = ctx.SrmValLineCount,
            ["SrmValLineAgeDays"] = ctx.SrmValLineAgeDays,
            ["SrmValLineFailCount"] = ctx.SrmValLineFailCount,
            ["SrmValLineFailAgeDays"] = ctx.SrmValLineFailAgeDays,
            ["SrmValEmailCount"] = ctx.SrmValEmailCount,
            ["SrmValEmailAgeDays"] = ctx.SrmValEmailAgeDays,
            ["AfuCount"] = ctx.AfuCount,
            ["AfuAgeHours"] = ctx.AfuAgeHours,
            ["CsCount"] = ctx.CsCount,
            ["CsAgeHours"] = ctx.CsAgeHours,
            ["OcmNiprReadyCount"] = ctx.OcmNiprReadyCount,
            ["OcmNiprReadyAgeHours"] = ctx.OcmNiprReadyAgeHours,
            ["OcmSiprReadyCount"] = ctx.OcmSiprReadyCount,
            ["OcmSiprReadyAgeHours"] = ctx.OcmSiprReadyAgeHours,
            ["OcmNiprHoldCount"] = ctx.OcmNiprHoldCount,
            ["OcmNiprHoldAgeHours"] = ctx.OcmNiprHoldAgeHours,
            ["OcmSiprHoldCount"] = ctx.OcmSiprHoldCount,
            ["OcmSiprHoldAgeHours"] = ctx.OcmSiprHoldAgeHours,
            ["OcmNiprFatalCount"] = ctx.OcmNiprFatalCount,
            ["OcmNiprFatalAgeHours"] = ctx.OcmNiprFatalAgeHours,
            ["OcmSiprFatalCount"] = ctx.OcmSiprFatalCount,
            ["OcmSiprFatalAgeHours"] = ctx.OcmSiprFatalAgeHours,

            // RDM
            ["RdmUsnCount"] = ctx.RdmUsnCount,
            ["RdmUsnAgeDays"] = ctx.RdmUsnAgeDays,
            ["RdmEsdUsnCount"] = ctx.RdmEsdUsnCount,
            ["RdmEsdUsnAgeDays"] = ctx.RdmEsdUsnAgeDays,

            // Notes
            ["FocusArea"] = ctx.FocusArea,
            ["CirImpactAsa"] = ctx.CirImpactAsa,
            ["ImpactEvents"] = ctx.ImpactEvents,
            ["HpsmStatus"] = ctx.HpsmStatus,
            ["ManagementNotes"] = ctx.ManagementNotes
        };

        var subjectTemplate = string.IsNullOrWhiteSpace(tpl.Subject)
            ? "Interval Summary – {{Date:MMMM d, yyyy}} ({{Interval}})"
            : tpl.Subject;

        var subject = TemplateRenderer.Render(subjectTemplate, tokens, culture);

        // If template's Body is empty, build an HTML body from the page state (close to your “old” layout)
        var bodyTemplate = string.IsNullOrWhiteSpace(tpl.Body) ? DefaultBodyTemplate : tpl.Body!;
        var bodyHtml = TemplateRenderer.Render(bodyTemplate, tokens, culture);

        return (subject, bodyHtml);
    }

    // Tokenized HTML body that mirrors your old email content
    public static readonly string DefaultBodyTemplate = @"
<p><b>Alcon,</b></p>

<h4>Highlights</h4>
<p><b>Impacting Events:</b> {{ImpactEvents}}<br/>
<b>HPSM Status:</b> {{HpsmStatus}}</p>

<h4>Daily Stats</h4>
<p><b>SvD Total Inbound Calls Today</b><br/>
Calls Offered: {{UsnCallsOffered}}<br/>
Calls Handled: {{UsnCallsAnswered}}<br/>
Daily ASA: {{UsnAsa}} sec</p>

<p><b>VIP</b><br/>
Calls Offered: {{VipCallsOffered}}<br/>
Calls Handled: {{VipCallsAnswered}}<br/>
Daily ASA: {{VipAsa}} sec</p>

<p><b>SIPR Stats</b><br/>
Calls Offered: {{SiprCallsOffered}}<br/>
Calls Handled: {{SiprCallsAnswered}}<br/>
Daily ASA: {{SiprAsa}} sec</p>

<p><b>NNPI Stats</b><br/>
Calls Offered: {{NnpiCallsOffered}}<br/>
Calls Handled: {{NnpiCallsAnswered}}<br/>
Daily ASA: {{NnpiAsa}} sec</p>

<h4>Month-To-Date Stats</h4>
<p><b>SvD Total Inbound Calls MTD</b><br/>
Calls Offered: {{MtdUsnCallsOffered}}<br/>
Calls Handled: {{MtdUsnCallsAnswered}}<br/>
MTD ASA: {{MtdUsnAsa}} sec</p>

<p><b>VIP</b><br/>
Calls Offered: {{MtdVipCallsOffered}}<br/>
Calls Handled: {{MtdVipCallsAnswered}}<br/>
MTD ASA: {{MtdVipAsa}} sec</p>

<p><b>SIPR Stats</b><br/>
Calls Offered: {{MtdSiprCallsOffered}}<br/>
Calls Handled: {{MtdSiprCallsAnswered}}<br/>
MTD ASA: {{MtdSiprAsa}} sec</p>

<p><b>NNPI Stats</b><br/>
Calls Offered: {{MtdNnpiCallsOffered}}<br/>
Calls Handled: {{MtdNnpiCallsAnswered}}<br/>
MTD ASA: {{MtdNnpiAsa}} sec</p>

<h4>E-mail</h4>
<p>EM MTD SLR (LOS1): {{Slr33EmLos1:0.00}} %<br/>
EM MTD SLR (LOS2): {{Slr33EmLos2:0.00}} %<br/>
EM Inbox Count: {{EmailCount}}<br/>
Oldest EM (3hr SLR): {{EmailOldestHours}} hrs<br/>
Cust Care Count: {{CustCareCount}}<br/>
Oldest Cust Care (24hr SLR): {{CustCareOldestHours}} hrs<br/>
SOC EM Inbox Count: {{SiprEmailCount}}<br/>
Oldest SOC EM: {{SiprEmailOldestHours}} hrs<br/>
SIPR GDA Spreadsheets: {{SiprGdaCount}} items<br/>
Oldest SIPR Spreadsheet: {{SiprGdaOldestHours}} hrs<br/>
SIPR UAIF count: {{SiprUaifCount}} forms<br/>
Oldest SIPR UAIF: {{SiprUaifOldestDays}} days</p>

<h4>Voicemail</h4>
<p>VM MTD SLR (LOS1): {{Slr33VmLos1:0.00}} %<br/>
VM MTD SLR (LOS2): {{Slr33VmLos2:0.00}} %<br/>
VM Inbox Count: {{VmCount}}<br/>
Oldest VM: {{VmOldestHours}} hrs</p>

<h4>Enterprise Self Service</h4>
<p>ESS Interaction Count: {{EssCount}}<br/>
Oldest ESS Interaction: {{EssOldestHours}} hrs</p>

<h4>Backlog</h4>
<p><b>Service Request Management Queues</b><br/>
SRM User Admin<br/>
&nbsp;&nbsp;Auto USN: {{SrmAutoCount}} &nbsp;&nbsp; Oldest: {{SrmAutoAgeHours}} Hours<br/>
&nbsp;&nbsp;Manual USN: {{SrmUsnManCount}} &nbsp;&nbsp; Oldest: {{SrmUsnManAgeHours}} Hours<br/>
&nbsp;&nbsp;Manual SOC: {{SrmSocManCount}} &nbsp;&nbsp; Oldest: {{SrmSocManAgeHours}} Hours<br/>
SRM Validation<br/>
&nbsp;&nbsp;Line Items: {{SrmValLineCount}} &nbsp;&nbsp; Oldest: {{SrmValLineAgeDays}} Days<br/>
&nbsp;&nbsp;Failed Inbound: {{SrmValLineFailCount}} &nbsp;&nbsp; Oldest: {{SrmValLineFailAgeDays}} Days<br/>
&nbsp;&nbsp;Email Buildouts: {{SrmValEmailCount}} &nbsp;&nbsp; Oldest: {{SrmValEmailAgeDays}} Days<br/>
SRM Active Follow up (Automated): {{AfuCount}} &nbsp;&nbsp; Oldest: {{AfuAgeHours}} Hours<br/>
SRM Customer Satisfaction: {{CsCount}} &nbsp;&nbsp; Oldest: {{CsAgeHours}} Days<br/>
SRM OCM Account Activation Buildouts in Queue<br/>
&nbsp;&nbsp;NIPR OCM Ready: {{OcmNiprReadyCount}} &nbsp;&nbsp; Oldest: {{OcmNiprReadyAgeHours}} Hours<br/>
&nbsp;&nbsp;NIPR OCM Hold: {{OcmNiprHoldCount}} &nbsp;&nbsp; Oldest: {{OcmNiprHoldAgeHours}} Hours<br/>
&nbsp;&nbsp;NIPR Fatal Review: {{OcmNiprFatalCount}} &nbsp;&nbsp; Oldest: {{OcmNiprFatalAgeHours}} Hours<br/>
&nbsp;&nbsp;SIPR OCM Ready: {{OcmSiprReadyCount}} &nbsp;&nbsp; Oldest: {{OcmSiprReadyAgeHours}} Hours<br/>
&nbsp;&nbsp;SIPR OCM Hold: {{OcmSiprHoldCount}} &nbsp;&nbsp; Oldest: {{OcmSiprHoldAgeHours}} Hours<br/>
&nbsp;&nbsp;SIPR Fatal Review: {{OcmSiprFatalCount}} &nbsp;&nbsp; Oldest: {{OcmSiprFatalAgeHours}} Hours</p>

<h4>Remote Desktop Management HPSM Queue Status</h4>
<p>RDM USN: {{RdmUsnCount}} &nbsp;&nbsp; Oldest: {{RdmUsnAgeDays}} Days<br/>
ESD USN: {{RdmEsdUsnCount}} &nbsp;&nbsp; Oldest: {{RdmEsdUsnAgeDays}} Days</p>

<h4>Notes / Actions</h4>
<p><b>Today's Focus Areas:</b> {{FocusArea}}<br/>
<b>Major CIRs impacting ASA:</b> {{CirImpactAsa}}<br/>
<b>Management Notes:</b> {{ManagementNotes}}</p>

<p><i>Interval:</i> {{Date:MMMM d, yyyy}} ({{Interval}})</p>";
}
