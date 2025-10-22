using Dapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using MimeKit; // .eml creation
using MudBlazor;
using MyApplication.Common;                // IdentityHelpers
using MyApplication.Common.Time;           // Et helper
using MyApplication.Components.Pages.Tools.Interval;
using MyApplication.Components.Service;
using MyApplication.Components.Services.Email;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyApplication.Components.Pages.Tools.Interval
{
    public partial class IntervalSummary : ComponentBase, IDisposable
    {
        [Inject] private IConfiguration Config { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IEmailComposer EmailComposer { get; set; } = default!;
        [Inject] private IntervalEmailService EmailSvc { get; set; } = default!;
        [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = default!;

        private IntervalSummaryState State { get; set; } = new();

        // Connection strings
        private string _connString = string.Empty;   // legacy usage (AWS-first)
        private string _connAws = string.Empty;      // AWS call data
        private string _connAom = string.Empty;      // AOM app DB (IntervalSummary table)

        private string _lastError = string.Empty;
        private string? _info;
        private readonly CancellationTokenSource _cts = new();
        private bool _disposed;

        // ======== Interval grid model (right-hand table) ========
        private sealed class IntervalGridRow
        {
            public string IntervalLabel { get; set; } = string.Empty; // "HH:mm - HH:mm"
            public int CallsOffered { get; set; }
            public int Answered { get; set; }
            public decimal ASA { get; set; } // seconds, 2 decimals
        }
        private List<IntervalGridRow> _intervalRows = new();

        protected override void OnInitialized()
        {
            InitConnStrings();

            if (string.IsNullOrWhiteSpace(_connAws) && string.IsNullOrWhiteSpace(_connAom))
            {
                _lastError = "No SQL connection strings found. Expected ConnectionStrings:AWS and/or ConnectionStrings:AOM.";
                Snackbar.Add(_lastError, Severity.Error, cfg => cfg.RequireInteraction = true);
            }

            // Use global ET helper
            var etNowMinus30 = Et.Now.AddMinutes(-30);
            var startHour = (etNowMinus30.Hour / 4) * 4;
            var endHour = startHour + 4;

            State.Header.IntervalDate = etNowMinus30.Date;
            State.Header.IntervalStart = $"{startHour:00}:00";
            State.Header.IntervalEnd = $"{endHour:00}:00";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await PopulateAsync();
        }

        // Initialize both connection strings,
        // keep _connString behavior for existing WithConn() usage (AWS-first, fallback AOM)
        private void InitConnStrings()
        {
            _connAws = Config.GetConnectionString("AWS") ?? Config["ConnectionStrings:AWS"] ?? "";
            _connAom = Config.GetConnectionString("AOM") ?? Config["ConnectionStrings:AOM"] ?? "";

            if (!string.IsNullOrWhiteSpace(_connAws))
            {
                _connString = _connAws;
                _info = string.IsNullOrWhiteSpace(_connAom)
                    ? "Using connection string: AWS"
                    : "Using connection strings: AWS + AOM";
            }
            else if (!string.IsNullOrWhiteSpace(_connAom))
            {
                _connString = _connAom; // to avoid nulls if someone calls WithConn by mistake
                _info = "Using connection string: AOM only (no AWS).";
            }
            else
            {
                _connString = string.Empty;
                _info = "No connection string found (looked for AWS, then AOM).";
            }
        }

        // ========================= Toolbar actions =========================
        private bool _loading;

        private async Task PopulateAsync()
        {
            if (string.IsNullOrWhiteSpace(_connAws) && string.IsNullOrWhiteSpace(_connAom))
            {
                _lastError = "Populate aborted: no AWS/AOM connection strings.";
                Snackbar.Add(_lastError, Severity.Error, cfg => cfg.RequireInteraction = true);
                return;
            }

            if (_loading) return;
            _loading = true;
            _lastError = string.Empty;

            try
            {
                // selectedDate is ET (date only)
                var selectedDate = (State.Header.IntervalDate ?? Et.Now).Date;

                var dayStart = selectedDate;           // 00:00 ET
                var dayEnd = selectedDate.AddDays(1);  // next day 00:00 ET

                var (mStart, mEnd) = GetMtdBounds(selectedDate); // both ET

                // Kick off all queries in parallel. Each uses its own connection.
                // Current Day (AWS call data)
                var tCurSvd = WithConn(c => QueryCurrentAsync(c, "isreportingweb = 1", dayStart, dayEnd, _cts.Token), _cts.Token);
                var tCurVip = WithConn(c => QueryCurrentAsync(c, "CMS_Equivalent = 46", dayStart, dayEnd, _cts.Token), _cts.Token);
                var tCurSipr = WithConn(c => QueryCurrentAsync(c, "CMS_Equivalent = 68", dayStart, dayEnd, _cts.Token), _cts.Token);
                var tCurNnpi = WithConn(c => QueryCurrentAsync(c, "CMS_Equivalent = 27", dayStart, dayEnd, _cts.Token), _cts.Token);

                // MTD (AWS call data)
                var tMtdSvd = WithConn(c => QueryMtdAsync(c, "isreportingweb = 1", mStart, mEnd, _cts.Token), _cts.Token);
                var tMtdVip = WithConn(c => QueryMtdAsync(c, "CMS_Equivalent = 46", mStart, mEnd, _cts.Token), _cts.Token);
                var tMtdSipr = WithConn(c => QueryMtdAsync(c, "CMS_Equivalent = 68", mStart, mEnd, _cts.Token), _cts.Token);
                var tMtdNnpi = WithConn(c => QueryMtdAsync(c, "CMS_Equivalent = 27", mStart, mEnd, _cts.Token), _cts.Token);

                // Intervals (AWS call data)
                var tIntervals = WithConn(c => QueryCurrentDayIntervalsAsync(c, dayStart, dayEnd, _cts.Token), _cts.Token);

                // Latest Notes (AOM DB)
                var tNotes = !string.IsNullOrWhiteSpace(_connAom)
                    ? WithConnAom(c => QueryLatestNotesAsync(c, _cts.Token), _cts.Token)
                    : Task.FromResult<LatestNotesRow?>(null);

                await Task.WhenAll(
                    tCurSvd, tCurVip, tCurSipr, tCurNnpi,
                    tMtdSvd, tMtdVip, tMtdSipr, tMtdNnpi,
                    tIntervals, tNotes
                );

                // Assign results (Current)
                var curSvd = tCurSvd.Result; var mtdSvd = tMtdSvd.Result;
                var curVip = tCurVip.Result; var mtdVip = tMtdVip.Result;
                var curSipr = tCurSipr.Result; var mtdSipr = tMtdSipr.Result;
                var curNnpi = tCurNnpi.Result; var mtdNnpi = tMtdNnpi.Result;

                State.Current.UsnAsa = curSvd.ASA.ToString(CultureInfo.InvariantCulture);
                State.Current.UsnCallsOffered = curSvd.CallsOffered.ToString(CultureInfo.InvariantCulture);
                State.Current.UsnCallsAnswered = curSvd.Answered.ToString(CultureInfo.InvariantCulture);

                State.Current.VipAsa = curVip.ASA.ToString(CultureInfo.InvariantCulture);
                State.Current.VipCallsOffered = curVip.CallsOffered.ToString(CultureInfo.InvariantCulture);
                State.Current.VipCallsAnswered = curVip.Answered.ToString(CultureInfo.InvariantCulture);

                State.Current.SiprAsa = curSipr.ASA.ToString(CultureInfo.InvariantCulture);
                State.Current.SiprCallsOffered = curSipr.CallsOffered.ToString(CultureInfo.InvariantCulture);
                State.Current.SiprCallsAnswered = curSipr.Answered.ToString(CultureInfo.InvariantCulture);

                State.Current.NnpiAsa = curNnpi.ASA.ToString(CultureInfo.InvariantCulture);
                State.Current.NnpiCallsOffered = curNnpi.CallsOffered.ToString(CultureInfo.InvariantCulture);
                State.Current.NnpiCallsAnswered = curNnpi.Answered.ToString(CultureInfo.InvariantCulture);

                // Assign results (MTD)
                State.Mtd.UsnAsa = mtdSvd.ASA.ToString(CultureInfo.InvariantCulture);
                State.Mtd.UsnCallsOffered = mtdSvd.CallsOffered.ToString(CultureInfo.InvariantCulture);
                State.Mtd.UsnCallsAnswered = mtdSvd.Answered.ToString(CultureInfo.InvariantCulture);

                State.Mtd.VipAsa = mtdVip.ASA.ToString(CultureInfo.InvariantCulture);
                State.Mtd.VipCallsOffered = mtdVip.CallsOffered.ToString(CultureInfo.InvariantCulture);
                State.Mtd.VipCallsAnswered = mtdVip.Answered.ToString(CultureInfo.InvariantCulture);

                State.Mtd.SiprAsa = mtdSipr.ASA.ToString(CultureInfo.InvariantCulture);
                State.Mtd.SiprCallsOffered = mtdSipr.CallsOffered.ToString(CultureInfo.InvariantCulture);
                State.Mtd.SiprCallsAnswered = mtdSipr.Answered.ToString(CultureInfo.InvariantCulture);

                State.Mtd.NnpiAsa = mtdNnpi.ASA.ToString(CultureInfo.InvariantCulture);
                State.Mtd.NnpiCallsOffered = mtdNnpi.CallsOffered.ToString(CultureInfo.InvariantCulture);
                State.Mtd.NnpiCallsAnswered = mtdNnpi.Answered.ToString(CultureInfo.InvariantCulture);

                // Intervals
                _intervalRows = tIntervals.Result;

                // Latest Notes
                var latest = tNotes.Result;
                if (latest is not null)
                {
                    State.Notes.FocusArea = latest.NaTodaysFocusArea ?? "";
                    State.Notes.CirImpactAsa = latest.NaMajorCirImpact ?? "";
                    State.Notes.ImpactEvents = latest.NaImpactingEvents ?? "";
                    State.Notes.HpsmStatus = latest.NaHpsmStatus ?? "";
                    State.Notes.ManagementNotes = latest.NaManagementNotes ?? "";
                }

                await SafeStateHasChangedAsync();
                Snackbar.Add($"Populate complete for {selectedDate:yyyy-MM-dd}. Intervals: {_intervalRows.Count}.", Severity.Success);
            }
            catch (TaskCanceledException) { }
            catch (JSDisconnectedException) { }
            catch (ObjectDisposedException) { }
            catch (SqlException ex)
            {
                _lastError = $"SQL error during Populate: {ex.Message}";
                Console.WriteLine(ex);
                Snackbar.Add(_lastError, Severity.Error, cfg => cfg.RequireInteraction = true);
            }
            catch (Exception ex)
            {
                _lastError = $"Unexpected error during Populate: {ex.Message}";
                Console.WriteLine(ex);
                Snackbar.Add(_lastError, Severity.Error, cfg => cfg.RequireInteraction = true);
            }
            finally
            {
                _loading = false;
            }
        }

        private string Triplet(string label, string asa, string offered, string answered)
            => $"{label}: ASA {asa}s | Offered {offered} | Handled {answered}";

        private async Task CopyStatsAsync()
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Current Day");
                sb.AppendLine(Triplet("SvD", State.Current.UsnAsa, State.Current.UsnCallsOffered, State.Current.UsnCallsAnswered));
                sb.AppendLine(Triplet("VIP", State.Current.VipAsa, State.Current.VipCallsOffered, State.Current.VipCallsAnswered));
                sb.AppendLine(Triplet("SIPR", State.Current.SiprAsa, State.Current.SiprCallsOffered, State.Current.SiprCallsAnswered));
                sb.AppendLine(Triplet("NNPI", State.Current.NnpiAsa, State.Current.NnpiCallsOffered, State.Current.NnpiCallsAnswered));
                sb.AppendLine();

                sb.AppendLine("Month To Date");
                sb.AppendLine(Triplet("SvD", State.Mtd.UsnAsa, State.Mtd.UsnCallsOffered, State.Mtd.UsnCallsAnswered));
                sb.AppendLine(Triplet("VIP", State.Mtd.VipAsa, State.Mtd.VipCallsOffered, State.Mtd.VipCallsAnswered));
                sb.AppendLine(Triplet("SIPR", State.Mtd.SiprAsa, State.Mtd.SiprCallsOffered, State.Mtd.SiprCallsAnswered));
                sb.AppendLine(Triplet("NNPI", State.Mtd.NnpiAsa, State.Mtd.NnpiCallsOffered, State.Mtd.NnpiCallsAnswered));

                await JS.InvokeVoidAsync("copyToClipboard", sb.ToString());
                Snackbar.Add("Stats copied to clipboard.", Severity.Success);
            }
            catch (JSDisconnectedException) { }
            catch (JSException)
            {
                Snackbar.Add("Clipboard blocked by browser (requires a user gesture/HTTPS).", Severity.Warning);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Copy failed: {ex.Message}", Severity.Error);
            }
        }

        private async Task GenerateEmailAsync()
        {
            try
            {
                // Prepared By
                var auth = await AuthStateTask;
                var preparedBy = IdentityHelpers.GetSamAccount(auth.User);

                // Build the same context you were already using
                var ctx = BuildIntervalContextFromState();

                // Compose and log (your existing service)
                var (subject, html, to, cc, from) = await EmailSvc.ComposeAndLogAsync(ctx, _cts.Token);

                // ---- helpers: robust HH:mm / HHmm no matter the type (DateTime, TimeSpan, string) ----
                static string ToHHmm(object? t)
                {
                    switch (t)
                    {
                        case DateTime dt: return dt.ToString("HHmm", CultureInfo.InvariantCulture);
                        case DateTimeOffset dto: return dto.ToString("HHmm", CultureInfo.InvariantCulture);
                        case TimeSpan ts: return ts.ToString(@"hhmm", CultureInfo.InvariantCulture);
                        case string s:
                            if (string.IsNullOrWhiteSpace(s)) return "0000";
                            var digits = new string(s.Where(char.IsDigit).ToArray());
                            return digits.Length >= 4 ? digits[..4] : digits.PadRight(4, '0');
                        default: return "0000";
                    }
                }

                static string ToHH_colon_mm(object? t)
                {
                    switch (t)
                    {
                        case DateTime dt: return dt.ToString("HH':'mm", CultureInfo.InvariantCulture);
                        case DateTimeOffset dto: return dto.ToString("HH':'mm", CultureInfo.InvariantCulture);
                        case TimeSpan ts: return ts.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
                        case string s:
                            if (string.IsNullOrWhiteSpace(s)) return "00:00";
                            var digits = new string(s.Where(char.IsDigit).ToArray());
                            digits = digits.Length >= 4 ? digits[..4] : digits.PadRight(4, '0');
                            return $"{digits[..2]}:{digits[2..4]}";
                        default: return "00:00";
                    }
                }

                var startHHmm = ToHHmm(ctx.IntervalStart);
                var endHHmm = ToHHmm(ctx.IntervalEnd);
                var startHH_colon = ToHH_colon_mm(ctx.IntervalStart);
                var endHH_colon = ToHH_colon_mm(ctx.IntervalEnd);

                // Subject with interval (24-hour, avoids invalid format on strings)
                var subjectWithInterval = $"{subject} — {ctx.DateLocal:yyyy-MM-dd} {startHH_colon}-{endHH_colon} ET";

                // ---- Build draft ----
                var draft = new EmailDraft
                {
                    Subject = subjectWithInterval,
                    HtmlBody = html,
                    From = from?.Trim(),           // shared mailbox SMTP
                    To = to ?? string.Empty,
                    Cc = cc ?? string.Empty,
                    OpenAsDraft = true,
                    IncludeCui = true
                }
                // ET-aware banner (CUI for Interval)
                .WithCuiBanner("Interval Summary", preparedBy, generatedEt: Et.Now);

                // Build message payload and download as .oft so Outlook opens a compose window
                var bytes = EmailDraftBuilder.BuildMsgBytes(draft);
                var base64 = Convert.ToBase64String(bytes);
                var fileName = $"IntervalSummary_{ctx.DateLocal:yyyyMMdd}_{startHHmm}-{endHHmm}.oft";

                await JS.InvokeVoidAsync(
                    "downloadFileFromBase64",
                    fileName,
                    "application/vnd.ms-outlook",
                    base64
                );

                Snackbar.Add("Draft generated — check your downloads.", Severity.Success);
            }
            catch (JSDisconnectedException) { /* ignore navigation race */ }
            catch (Exception ex)
            {
                Snackbar.Add($"Could not generate email: {ex.Message}", Severity.Error);
            }
        }

        // Build Interval email context from current UI state
        private IntervalEmailContext BuildIntervalContextFromState()
        {
            static int I(string? s) =>
                int.TryParse(s, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var v) ? v : 0;

            static double F(string? s) =>
                double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var v) ? v : 0d;

            static TimeSpan TS(string? s) =>
                TimeSpan.TryParseExact(s?.Trim(), new[] { "h\\:mm", "hh\\:mm" }, CultureInfo.InvariantCulture, out var t) ? t : TimeSpan.Zero;

            // ET date, not server local
            var dateLocal = (State.Header.IntervalDate ?? Et.Now).Date;

            return new IntervalEmailContext
            {
                DateLocal = dateLocal,
                IntervalStart = TS(State.Header.IntervalStart),
                IntervalEnd = TS(State.Header.IntervalEnd),

                // Current Day — SvD/USN
                UsnAsa = F(State.Current.UsnAsa),
                UsnCallsOffered = I(State.Current.UsnCallsOffered),
                UsnCallsAnswered = I(State.Current.UsnCallsAnswered),

                // Current Day — VIP
                VipAsa = F(State.Current.VipAsa),
                VipCallsOffered = I(State.Current.VipCallsOffered),
                VipCallsAnswered = I(State.Current.VipCallsAnswered),

                // Current Day — SIPR
                SiprAsa = F(State.Current.SiprAsa),
                SiprCallsOffered = I(State.Current.SiprCallsOffered),
                SiprCallsAnswered = I(State.Current.SiprCallsAnswered),

                // Current Day — NNPI
                NnpiAsa = F(State.Current.NnpiAsa),
                NnpiCallsOffered = I(State.Current.NnpiCallsOffered),
                NnpiCallsAnswered = I(State.Current.NnpiCallsAnswered),

                // Month To Date
                MtdUsnAsa = F(State.Mtd.UsnAsa),
                MtdUsnCallsOffered = I(State.Mtd.UsnCallsOffered),
                MtdUsnCallsAnswered = I(State.Mtd.UsnCallsAnswered),

                MtdVipAsa = F(State.Mtd.VipAsa),
                MtdVipCallsOffered = I(State.Mtd.VipCallsOffered),
                MtdVipCallsAnswered = I(State.Mtd.VipCallsAnswered),

                MtdSiprAsa = F(State.Mtd.SiprAsa),
                MtdSiprCallsOffered = I(State.Mtd.SiprCallsOffered),
                MtdSiprCallsAnswered = I(State.Mtd.SiprCallsAnswered),

                MtdNnpiAsa = F(State.Mtd.NnpiAsa),
                MtdNnpiCallsOffered = I(State.Mtd.NnpiCallsOffered),
                MtdNnpiCallsAnswered = I(State.Mtd.NnpiCallsAnswered),

                // SLR33 (MTD %)
                Slr33EmLos1 = F(State.Mtd.Slr33EmLos1),
                Slr33EmLos2 = F(State.Mtd.Slr33EmLos2),
                Slr33VmLos1 = F(State.Mtd.Slr33VmLos1),
                Slr33VmLos2 = F(State.Mtd.Slr33VmLos2),

                // Email / Cust Care / SIPR Email / GDA / UAIF
                EmailCount = I(State.Current.EmailCount),
                EmailOldestHours = F(State.Current.EmailOldestHours),
                CustCareCount = I(State.Current.CustCareCount),
                CustCareOldestHours = F(State.Current.CustCareOldestHours),
                SiprEmailCount = I(State.Current.SiprEmailCount),
                SiprEmailOldestHours = F(State.Current.SiprEmailOldestHours),
                SiprGdaCount = I(State.Current.SiprGdaCount),
                SiprGdaOldestHours = F(State.Current.SiprGdaOldestHours),
                SiprUaifCount = I(State.Current.SiprUaifCount),
                SiprUaifOldestDays = F(State.Current.SiprUaifOldestDays),

                // VM / ESS
                VmCount = I(State.Current.VmCount),
                VmOldestHours = F(State.Current.VmOldestHours),
                EssCount = I(State.Current.EssCount),
                EssOldestHours = F(State.Current.EssOldestHours),

                // Backlog — SRM User Admin
                SrmAutoCount = I(State.Backlog.SrmAutoCount),
                SrmAutoAgeHours = F(State.Backlog.SrmAutoAgeHours),
                SrmUsnManCount = I(State.Backlog.SrmUsnManCount),
                SrmUsnManAgeHours = F(State.Backlog.SrmUsnManAgeHours),
                SrmSocManCount = I(State.Backlog.SrmSocManCount),
                SrmSocManAgeHours = F(State.Backlog.SrmSocManAgeHours),

                // Backlog — SRM Validation
                SrmValLineCount = I(State.Backlog.SrmValLineCount),
                SrmValLineAgeDays = F(State.Backlog.SrmValLineAgeDays),
                SrmValLineFailCount = I(State.Backlog.SrmValLineFailCount),
                SrmValLineFailAgeDays = F(State.Backlog.SrmValLineFailAgeDays),
                SrmValEmailCount = I(State.Backlog.SrmValEmailCount),
                SrmValEmailAgeDays = F(State.Backlog.SrmValEmailAgeDays),

                // Backlog — AFU / Incidents
                AfuCount = I(State.Backlog.AfuCount),
                AfuAgeHours = F(State.Backlog.AfuAgeHours),
                CsCount = I(State.Backlog.CsCount),
                CsAgeHours = F(State.Backlog.CsAgeHours),

                // Backlog — OCM
                OcmNiprReadyCount = I(State.Backlog.OcmNiprReadyCount),
                OcmNiprReadyAgeHours = F(State.Backlog.OcmNiprReadyAgeHours),
                OcmSiprReadyCount = I(State.Backlog.OcmSiprReadyCount),
                OcmSiprReadyAgeHours = F(State.Backlog.OcmSiprReadyAgeHours),

                OcmNiprHoldCount = I(State.Backlog.OcmNiprHoldCount),
                OcmNiprHoldAgeHours = F(State.Backlog.OcmNiprHoldAgeHours),
                OcmSiprHoldCount = I(State.Backlog.OcmSiprHoldCount),
                OcmSiprHoldAgeHours = F(State.Backlog.OcmSiprHoldAgeHours),

                OcmNiprFatalCount = I(State.Backlog.OcmNiprFatalCount),
                OcmNiprFatalAgeHours = F(State.Backlog.OcmNiprFatalAgeHours),
                OcmSiprFatalCount = I(State.Backlog.OcmSiprFatalCount),
                OcmSiprFatalAgeHours = F(State.Backlog.OcmSiprFatalAgeHours),

                // RDM
                RdmUsnCount = I(State.Backlog.RdmUsnCount),
                RdmUsnAgeDays = F(State.Backlog.RdmUsnAgeDays),
                RdmEsdUsnCount = I(State.Backlog.RdmEsdUsnCount),
                RdmEsdUsnAgeDays = F(State.Backlog.RdmEsdUsnAgeDays),

                // Notes
                FocusArea = State.Notes.FocusArea,
                CirImpactAsa = State.Notes.CirImpactAsa,
                ImpactEvents = State.Notes.ImpactEvents,
                HpsmStatus = State.Notes.HpsmStatus,
                ManagementNotes = State.Notes.ManagementNotes
            };
        }

        private async Task ClearAsync()
        {
            var keepHeader = State.Header;
            State = new IntervalSummaryState { Header = keepHeader };
            _intervalRows.Clear();
            await SafeStateHasChangedAsync();
        }

        // ========================= Data helpers (Dapper) =========================

        // Month-to-date bounds in ET; anchorEtLocal is already ET
        private static (DateTime start, DateTime end) GetMtdBounds(DateTime anchorEtLocal)
        {
            var first = new DateTime(anchorEtLocal.Year, anchorEtLocal.Month, 1);
            var start = first.Date;
            var end = start.AddMonths(1);
            return (start, end);
        }

        private sealed class AggregateRow
        {
            public int? ASA { get; set; }
            public int? CallsOffered { get; set; }
            public int? Answered { get; set; }
        }

        private async Task<(int ASA, int CallsOffered, int Answered)> QueryCurrentAsync(
            SqlConnection conn, string where, DateTime dayStart, DateTime dayEnd, CancellationToken ct)
        {
            var sql = $@"
SELECT 
    CAST(
        CASE WHEN SUM(ACDcalls + voicemails) > 0 
             THEN ROUND(SUM(CAST(anstime AS FLOAT)) / SUM(CAST(ACDcalls + voicemails AS FLOAT)), 0)
             ELSE 0 
        END AS INT
    ) AS ASA,
    SUM(callsoffered) AS CallsOffered,
    SUM(acdcalls + voicemails + callbacks) AS Answered
FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
WHERE {where}
  AND CAST(edate AS date) >= @DayStart
  AND CAST(edate AS date) <  @DayEnd
  AND initiationmethod = 'INBOUND';";

            var cmd = new CommandDefinition(
                sql,
                new { DayStart = dayStart.Date, DayEnd = dayEnd.Date },
                cancellationToken: ct,
                commandTimeout: 60);

            var row = await conn.QuerySingleOrDefaultAsync<AggregateRow>(cmd);
            if (row is null) return (0, 0, 0);
            return (row.ASA ?? 0, row.CallsOffered ?? 0, row.Answered ?? 0);
        }

        private async Task<(int ASA, int CallsOffered, int Answered)> QueryMtdAsync(
            SqlConnection conn, string where, DateTime monthStart, DateTime monthEnd, CancellationToken ct)
        {
            var sql = $@"
SELECT 
    CAST(
        CASE WHEN SUM(ACDcalls + voicemails) > 0 
             THEN ROUND(SUM(CAST(anstime AS FLOAT)) / SUM(CAST(ACDcalls + voicemails AS FLOAT)), 0)
             ELSE 0 
        END AS INT
    ) AS ASA,
    SUM(callsoffered) AS CallsOffered,
    SUM(acdcalls + voicemails + callbacks) AS Answered
FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
WHERE {where}
  AND CAST(edate AS date) >= @MonthStart
  AND CAST(edate AS date) <  @MonthEnd
  AND initiationmethod = 'INBOUND';";

            var cmd = new CommandDefinition(
                sql,
                new { MonthStart = monthStart.Date, MonthEnd = monthEnd.Date },
                cancellationToken: ct,
                commandTimeout: 60);

            var row = await conn.QuerySingleOrDefaultAsync<AggregateRow>(cmd);
            if (row is null) return (0, 0, 0);
            return (row.ASA ?? 0, row.CallsOffered ?? 0, row.Answered ?? 0);
        }

        private async Task<List<IntervalGridRow>> QueryCurrentDayIntervalsAsync(
            SqlConnection conn, DateTime dayStart, DateTime dayEnd, CancellationToken ct)
        {
            var sql = @"
SELECT
    CONVERT(varchar(5), [interval], 108) + ' - ' +
    CONVERT(varchar(5), DATEADD(minute, 29, [interval]), 108) AS IntervalLabel,
    SUM(callsoffered)                               AS CallsOffered,
    SUM(acdcalls + voicemails + callbacks)          AS Answered,
    CASE WHEN SUM(acdcalls + voicemails) > 0
         THEN CAST(SUM(CAST(anstime AS float)) / SUM(CAST(acdcalls + voicemails AS float)) AS decimal(10,2))
         ELSE 0
    END                                             AS ASA
FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
WHERE initiationmethod = 'INBOUND'
  AND isreportingweb = 1
  AND CAST(edate AS date) >= @DayStart
  AND CAST(edate AS date) <  @DayEnd
GROUP BY [interval]
ORDER BY [interval];";

            var cmd = new CommandDefinition(
                sql,
                new { DayStart = dayStart.Date, DayEnd = dayEnd.Date },
                cancellationToken: ct,
                commandTimeout: 60);

            var rows = await conn.QueryAsync<IntervalGridRow>(cmd);
            return rows.AsList();
        }

        // ====== Latest Notes from AOM ======
        private sealed class LatestNotesRow
        {
            public string? NaTodaysFocusArea { get; set; }
            public string? NaMajorCirImpact { get; set; }
            public string? NaImpactingEvents { get; set; }
            public string? NaHpsmStatus { get; set; }
            public string? NaManagementNotes { get; set; }
        }

        private async Task<LatestNotesRow?> QueryLatestNotesAsync(SqlConnection conn, CancellationToken ct)
        {
            const string sql = @"
SELECT TOP (1)
       [NaTodaysFocusArea],
       [NaMajorCirImpact],
       [NaImpactingEvents],
       [NaHpsmStatus],
       [NaManagementNotes]
FROM [Aom].[Tools].[IntervalSummary] WITH (NOLOCK)
ORDER BY [Id] DESC;";

            var cmd = new CommandDefinition(sql, cancellationToken: ct, commandTimeout: 60);
            return await conn.QuerySingleOrDefaultAsync<LatestNotesRow>(cmd);
        }

        private async Task<T> WithConn<T>(Func<SqlConnection, Task<T>> work, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_connAws) && string.IsNullOrWhiteSpace(_connString))
                throw new InvalidOperationException("AWS connection string is empty.");

            using var c = new SqlConnection(string.IsNullOrWhiteSpace(_connAws) ? _connString : _connAws);
            await c.OpenAsync(ct);
            return await work(c);
        }

        private async Task<T> WithConnAom<T>(Func<SqlConnection, Task<T>> work, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_connAom))
                throw new InvalidOperationException("AOM connection string is empty. Add ConnectionStrings:AOM.");
            using var c = new SqlConnection(_connAom);
            await c.OpenAsync(ct);
            return await work(c);
        }

        // ========================= Utilities =========================
        private async Task SafeStateHasChangedAsync()
        {
            if (_disposed) return;
            try { await InvokeAsync(StateHasChanged); }
            catch (JSDisconnectedException) { }
            catch (ObjectDisposedException) { }
            catch (TaskCanceledException) { }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _cts.Cancel(); } catch { }
            _cts.Dispose();
        }
    }
}
