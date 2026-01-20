// MyApplication/Components/Services/Email/IntervalEmailBuilder.cs
using System.Globalization;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Services.Email;
using System.Text.RegularExpressions;
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

            ["Slr33EmMtdLos1"] = ctx.Slr33EmMtdLos1,
            ["Slr33EmMtdLos2"] = ctx.Slr33EmMtdLos2,
            ["Slr33VmMtdLos1"] = ctx.Slr33VmMtdLos1,
            ["Slr33VmMtdLos2"] = ctx.Slr33VmMtdLos2,

            // Backlog
            ["CurrentEmailCount"] = ctx.CurrentEmailCount,
            ["CurrentEmailOldest"] = ctx.CurrentEmailOldest,
            ["CurrentCustomerCareCount"] = ctx.CurrentCustomerCareCount,
            ["CurrentCustomerCareOldest"] = ctx.CurrentCustomerCareOldest,
            ["CurrentSiprEmailCount"] = ctx.CurrentSiprEmailCount,
            ["CurrentSiprEmailOldest"] = ctx.CurrentSiprEmailOldest,
            ["CurrentSiprGdaSpreadsheets"] = ctx.CurrentSiprGdaSpreadsheets,
            ["CurrentSiprGdaOldest"] = ctx.CurrentSiprGdaOldest,
            ["CurrentSiprUaifCount"] = ctx.CurrentSiprUaifCount,
            ["CurrentSiprUaifOldest"] = ctx.CurrentSiprUaifOldest,

            ["CurrentVmCount"] = ctx.CurrentVmCount,
            ["CurrentVmOldest"] = ctx.CurrentVmOldest,
            ["CurrentEssCount"] = ctx.CurrentEssCount,
            ["CurrentEssAgeDays"] = ctx.CurrentEssOldest,

            ["SrmUaAutoCount"] = ctx.SrmUaAutoCount,
            ["SrmUaAutoAgeHours"] = ctx.SrmUaAutoAgeHours,
            ["SrmUaUsnManCount"] = ctx.SrmUaUsnManCount,
            ["SrmUaUsnManAgeHours"] = ctx.SrmUaUsnManAgeHours,
            ["SrmUaSocManCount"] = ctx.SrmUaSocManCount,
            ["SrmUaSocManAgeHours"] = ctx.SrmUaSocManAgeHours,

            ["SrmValCount"] = ctx.SrmValCount,
            ["SrmValAgeDays"] = ctx.SrmValAgeDays,
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

            ["RdmUsnCount"] = ctx.RdmUsnCount,
            ["RdmUsnAgeDays"] = ctx.RdmUsnAgeDays,
            ["RdmEsdUsnCount"] = ctx.RdmEsdUsnCount,
            ["RdmEsdUsnAgeDays"] = ctx.RdmEsdUsnAgeDays,

            // Other Backlog Queues
            ["NnpiQueue"] = ctx.NnpiQueue,
            ["SiprQueue"] = ctx.SiprQueue,
            ["NcisQueue"] = ctx.NcisQueue,
            ["VipQueue"] = ctx.VipQueue,
            ["RdmNnpiQueue"] = ctx.RdmNnpiQueue,
            ["RdmSiprQueue"] = ctx.RdmSiprQueue,

            // Notes - UPDATED
            ["FocusArea"] = ctx.NaTodaysFocusArea,
            ["CirImpactAsa"] = ctx.NaMajorCirImpact,
            ["ImpactEvents"] = ctx.NaImpactingEvents,
            ["HpsmStatus"] = ctx.NaHpsmStatus,
            ["ManagementNotes"] = ctx.NaManagementNotes
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

<h2>Highlights</h2>
<p><b>Impacting Events:</b> {{ImpactEvents}}<br/>
<b>HPSM Status:</b> {{HpsmStatus}}</p>

<h2>Daily Stats</h2>
<p><b>SvD Total Inbound Calls</b><br/>
Offered: {{UsnCallsOffered}}
&nbsp;&nbsp;  Handled: {{UsnCallsAnswered}}
&nbsp;&nbsp;  ASA: {{UsnAsa}} sec</p>

<p><b>VIP Inbound Calls</b><br/>
Offered: {{VipCallsOffered}}
&nbsp;&nbsp;  Handled: {{VipCallsAnswered}}
&nbsp;&nbsp;  ASA: {{VipAsa}} sec</p>

<p><b>SIPR Inbound Calls</b><br/>
Offered: {{SiprCallsOffered}}
&nbsp;&nbsp;  Handled: {{SiprCallsAnswered}}
&nbsp;&nbsp;  ASA: {{SiprAsa}} sec</p>

<p><b>NNPI Inbound Calls</b><br/>
Offered: {{NnpiCallsOffered}}
&nbsp;&nbsp;  Handled: {{NnpiCallsAnswered}}
&nbsp;&nbsp;  ASA: {{NnpiAsa}} sec</p>

<h2>Month-To-Date Stats</h2>
<p><b>SvD Total Inbound Calls</b><br/>
Offered: {{MtdUsnCallsOffered}}
&nbsp;&nbsp;  Handled: {{MtdUsnCallsAnswered}}
&nbsp;&nbsp;  ASA: {{MtdUsnAsa}} sec</p>

<p><b>VIP Inbound Calls</b><br/>
Offered: {{MtdVipCallsOffered}}
&nbsp;&nbsp;  Handled: {{MtdVipCallsAnswered}}
&nbsp;&nbsp;  ASA: {{MtdVipAsa}} sec</p>

<p><b>SIPR Inbound Calls</b><br/>
Offered: {{MtdSiprCallsOffered}}
&nbsp;&nbsp;  Handled: {{MtdSiprCallsAnswered}}
&nbsp;&nbsp;  ASA: {{MtdSiprAsa}} sec</p>

<p><b>NNPI Inbound Calls</b><br/>
Offered: {{MtdNnpiCallsOffered}}
&nbsp;&nbsp;  Handled: {{MtdNnpiCallsAnswered}}
&nbsp;&nbsp;  ASA: {{MtdNnpiAsa}} sec</p>

<p><b>E-mail Inbox</b><br/>
MTD LOS1: {{Slr33EmMtdLos1:0.00}} %
&nbsp;&nbsp; MTD LOS2: {{Slr33EmMtdLos2:0.00}} %<br/>
EM Inbox: {{CurrentEmailCount}}
&nbsp;&nbsp; Oldest: {{CurrentEmailOldest}} hrs<br/>
Cust Care: {{CurrentCustomerCareCount}}
&nbsp;&nbsp; Oldest: {{CurrentCustomerCareOldest}} hrs<br/></p>

<p><b>SIPR Stats</b><br/>
SOC EM Inbox: {{CurrentSiprEmailCount}}
&nbsp;&nbsp; Oldest: {{CurrentSiprEmailOldest}} hrs<br/>
SIPR GDA: {{CurrentSiprGdaSpreadsheets}} items
&nbsp;&nbsp; Oldest: {{CurrentSiprGdaOldest}} hrs<br/>
SIPR UAIF: {{CurrentSiprUaifCount}} forms
&nbsp;&nbsp; Oldest: {{CurrentSiprUaifOldest}} days</p>

<p><b>Voicemail Stats</b><br/>
MTD LOS1: {{Slr33VmMtdLos1:0.00}} %
&nbsp;&nbsp; MTD LOS2: {{Slr33VmMtdLos2:0.00}} %<br/>
VM Inbox: {{CurrentVmCount}}
&nbsp;&nbsp; Oldest: {{CurrentVmOldest}} hrs</p>

<p><b>Enterprise Self Service</b><br/>
ESS Interactions: {{CurrentEssCount}}
&nbsp;&nbsp; Oldest: {{CurrentEssAgeDays}} hrs</p>

<h2>Backlog</h2>

<h4>Service Request Management Queues</h4>
<p><b>SRM User Admin</b><br/>
Auto USN: {{SrmUaAutoCount}}   &nbsp;&nbsp; Oldest: {{SrmUaAutoAgeHours}} Days<br/>
Manual USN: {{SrmUaUsnManCount}}   &nbsp;&nbsp; Oldest: {{SrmUaUsnManAgeHours}} Days<br/>
Manual SOC: {{SrmUaSocManCount}}   &nbsp;&nbsp; Oldest: {{SrmUaSocManAgeHours}} Days</p>


<p><b>SRM Validation</b><br/>
Line Items: {{SrmValCount}} &nbsp;&nbsp; Oldest: {{SrmValAgeDays}} Days<br/>
Failed Inbound: {{SrmValLineFailCount}} &nbsp;&nbsp;  Oldest: {{SrmValLineFailAgeDays}} Days<br/>
Email Buildouts: {{SrmValEmailCount}}  &nbsp;&nbsp; Oldest: {{SrmValEmailAgeDays}} Days<br/>

<p><b>SRM Followup / Incidents</b><br/>
SRM Active Follow up: {{AfuCount}}  &nbsp;&nbsp;  Oldest: {{AfuAgeHours}} Days<br/>
SRM Incidents: {{CsCount}}  &nbsp;&nbsp;  Oldest: {{CsAgeHours}} Days</p>

<p><b>SRM OCM Activation</b><br/>
NIPR OCM Ready: {{OcmNiprReadyCount}} &nbsp;&nbsp;   Oldest: {{OcmNiprReadyAgeHours}} Hours<br/>
NIPR OCM Hold: {{OcmNiprHoldCount}}  &nbsp;&nbsp;  Oldest: {{OcmNiprHoldAgeHours}} Hours<br/>
NIPR Fatal Review: {{OcmNiprFatalCount}}   &nbsp;&nbsp;   Oldest: {{OcmNiprFatalAgeHours}} Hours<br/>
SIPR OCM Ready: {{OcmSiprReadyCount}}    &nbsp;&nbsp;  Oldest: {{OcmSiprReadyAgeHours}} Hours<br/>
SIPR OCM Hold: {{OcmSiprHoldCount}}    &nbsp;&nbsp;  Oldest: {{OcmSiprHoldAgeHours}} Hours<br/>
SIPR Fatal Review: {{OcmSiprFatalCount}}   &nbsp;&nbsp;   Oldest: {{OcmSiprFatalAgeHours}} Hours</p>

<p><b>Remote Desktop Management</b><br/>
RDM USN: {{RdmUsnCount}} &nbsp;&nbsp; Oldest: {{RdmUsnAgeDays}} Days<br/>
ESD USN: {{RdmEsdUsnCount}} &nbsp;&nbsp; Oldest: {{RdmEsdUsnAgeDays}} Days</p>

<p><b>Other HPSM Backlogs</b><br/>
SVD.NNPI: {{NnpiQueue}}
&nbsp;&nbsp; SVD.SIPR: {{SiprQueue}}  
&nbsp;&nbsp; SVD.NCIS: {{NcisQueue}} <br/>
SVD.VIP: {{VipQueue}} 
&nbsp;&nbsp; SVD.NNPI: {{RdmNnpiQueue}} 
&nbsp;&nbsp; SVD.SIPR: {{RdmSiprQueue}} 

<h4>Notes / Actions</h4>
<p><b>Today's Focus Areas:</b> {{FocusArea}} <br/>
<b>Major CIRs impacting ASA:</b> {{CirImpactAsa}} <br/>
<b>Management Notes:</b> {{ManagementNotes}} </p>";
}
