using Dapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using MimeKit;
using MudBlazor;
using MyApplication.Common;
using MyApplication.Common.Time;
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

        // Only AWS needed now
        private string _connAws = string.Empty;

        private string _lastError = string.Empty;
        private string? _info;
        private readonly CancellationTokenSource _cts = new();
        private bool _disposed;

        private sealed class IntervalGridRow
        {
            public string IntervalLabel { get; set; } = string.Empty;
            public int CallsOffered { get; set; }
            public int Answered { get; set; }
            public decimal ASA { get; set; }
        }
        private List<IntervalGridRow> _intervalRows = new();

        protected override void OnInitialized()
        {
            // Only load AWS connection string
            _connAws = Config.GetConnectionString("AWS") ?? Config["ConnectionStrings:AWS"] ?? "";

            if (string.IsNullOrWhiteSpace(_connAws))
            {
                _lastError = "No AWS connection string found.";
                Snackbar.Add(_lastError, Severity.Error, cfg => cfg.RequireInteraction = true);
            }
            else
            {
                _info = "Using connection string: AWS (AOM Disabled)";
            }

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

        private bool _loading;

        private async Task PopulateAsync()
        {
            if (string.IsNullOrWhiteSpace(_connAws))
            {
                _lastError = "Populate aborted: no AWS connection string.";
                Snackbar.Add(_lastError, Severity.Error, cfg => cfg.RequireInteraction = true);
                return;
            }

            if (_loading) return;
            _loading = true;
            _lastError = string.Empty;

            try
            {
                var selectedDate = (State.Header.IntervalDate ?? Et.Now).Date;
                var todayStart = selectedDate;
                var todayEnd = selectedDate.AddDays(1);
                var (monthStart, monthEnd) = GetMtdBounds(selectedDate);

                // Execute ONE round-trip query for everything
                var (stats, intervals) = await FetchEverythingAsync(todayStart, todayEnd, monthStart, monthEnd, _cts.Token);

                // --- Assign Current Day ---
                State.Current.UsnAsa = stats.Cur_UsnASA.ToString(CultureInfo.InvariantCulture);
                State.Current.UsnCallsOffered = stats.Cur_UsnOffered.ToString(CultureInfo.InvariantCulture);
                State.Current.UsnCallsAnswered = stats.Cur_UsnAnswered.ToString(CultureInfo.InvariantCulture);

                State.Current.VipAsa = stats.Cur_VipASA.ToString(CultureInfo.InvariantCulture);
                State.Current.VipCallsOffered = stats.Cur_VipOffered.ToString(CultureInfo.InvariantCulture);
                State.Current.VipCallsAnswered = stats.Cur_VipAnswered.ToString(CultureInfo.InvariantCulture);

                State.Current.SiprAsa = stats.Cur_SiprASA.ToString(CultureInfo.InvariantCulture);
                State.Current.SiprCallsOffered = stats.Cur_SiprOffered.ToString(CultureInfo.InvariantCulture);
                State.Current.SiprCallsAnswered = stats.Cur_SiprAnswered.ToString(CultureInfo.InvariantCulture);

                State.Current.NnpiAsa = stats.Cur_NnpiASA.ToString(CultureInfo.InvariantCulture);
                State.Current.NnpiCallsOffered = stats.Cur_NnpiOffered.ToString(CultureInfo.InvariantCulture);
                State.Current.NnpiCallsAnswered = stats.Cur_NnpiAnswered.ToString(CultureInfo.InvariantCulture);

                // --- Assign MTD ---
                State.Mtd.UsnAsa = stats.Mtd_UsnASA.ToString(CultureInfo.InvariantCulture);
                State.Mtd.UsnCallsOffered = stats.Mtd_UsnOffered.ToString(CultureInfo.InvariantCulture);
                State.Mtd.UsnCallsAnswered = stats.Mtd_UsnAnswered.ToString(CultureInfo.InvariantCulture);

                State.Mtd.VipAsa = stats.Mtd_VipASA.ToString(CultureInfo.InvariantCulture);
                State.Mtd.VipCallsOffered = stats.Mtd_VipOffered.ToString(CultureInfo.InvariantCulture);
                State.Mtd.VipCallsAnswered = stats.Mtd_VipAnswered.ToString(CultureInfo.InvariantCulture);

                State.Mtd.SiprAsa = stats.Mtd_SiprASA.ToString(CultureInfo.InvariantCulture);
                State.Mtd.SiprCallsOffered = stats.Mtd_SiprOffered.ToString(CultureInfo.InvariantCulture);
                State.Mtd.SiprCallsAnswered = stats.Mtd_SiprAnswered.ToString(CultureInfo.InvariantCulture);

                State.Mtd.NnpiAsa = stats.Mtd_NnpiASA.ToString(CultureInfo.InvariantCulture);
                State.Mtd.NnpiCallsOffered = stats.Mtd_NnpiOffered.ToString(CultureInfo.InvariantCulture);
                State.Mtd.NnpiCallsAnswered = stats.Mtd_NnpiAnswered.ToString(CultureInfo.InvariantCulture);

                // Intervals
                _intervalRows = intervals;

                await SafeStateHasChangedAsync();
                Snackbar.Add($"Populate complete for {selectedDate:yyyy-MM-dd}. Intervals: {_intervalRows.Count}.", Severity.Success);
            }
            catch (Exception ex)
            {
                _lastError = $"Error during Populate: {ex.Message}";
                Console.WriteLine(ex);
                Snackbar.Add(_lastError, Severity.Error, cfg => cfg.RequireInteraction = true);
            }
            finally
            {
                _loading = false;
            }
        }

        // ========================= Consolidated Data Access =========================

        private sealed class CombinedStatsRow
        {
            // Current Day
            public int Cur_UsnASA { get; set; }
            public int Cur_UsnOffered { get; set; }
            public int Cur_UsnAnswered { get; set; }
            public int Cur_VipASA { get; set; }
            public int Cur_VipOffered { get; set; }
            public int Cur_VipAnswered { get; set; }
            public int Cur_SiprASA { get; set; }
            public int Cur_SiprOffered { get; set; }
            public int Cur_SiprAnswered { get; set; }
            public int Cur_NnpiASA { get; set; }
            public int Cur_NnpiOffered { get; set; }
            public int Cur_NnpiAnswered { get; set; }

            // MTD
            public int Mtd_UsnASA { get; set; }
            public int Mtd_UsnOffered { get; set; }
            public int Mtd_UsnAnswered { get; set; }
            public int Mtd_VipASA { get; set; }
            public int Mtd_VipOffered { get; set; }
            public int Mtd_VipAnswered { get; set; }
            public int Mtd_SiprASA { get; set; }
            public int Mtd_SiprOffered { get; set; }
            public int Mtd_SiprAnswered { get; set; }
            public int Mtd_NnpiASA { get; set; }
            public int Mtd_NnpiOffered { get; set; }
            public int Mtd_NnpiAnswered { get; set; }
        }

        private async Task<(CombinedStatsRow, List<IntervalGridRow>)> FetchEverythingAsync(
            DateTime todayStart, DateTime todayEnd, DateTime monthStart, DateTime monthEnd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_connAws))
                throw new InvalidOperationException("AWS connection string is empty.");

            // This script has 2 statements:
            // 1. A single row containing ALL Current AND MTD aggregates (using conditional SUM)
            // 2. The list of interval rows for the Current Day
            var sql = @"
-- 1. Aggregates (Current + MTD in one scan)
SELECT 
    -- === CURRENT DAY (Filtered by @TodayStart) ===
    -- USN
    CAST(CASE WHEN SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN ACDcalls + voicemails ELSE 0 END) > 0 
         THEN ROUND(SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
         ELSE 0 END AS INT) as Cur_UsnASA,
    SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN callsoffered ELSE 0 END) as Cur_UsnOffered,
    SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN acdcalls + voicemails + callbacks ELSE 0 END) as Cur_UsnAnswered,

    -- VIP
    CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN ACDcalls + voicemails ELSE 0 END) > 0 
         THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
         ELSE 0 END AS INT) as Cur_VipASA,
    SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN callsoffered ELSE 0 END) as Cur_VipOffered,
    SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN acdcalls + voicemails + callbacks ELSE 0 END) as Cur_VipAnswered,

    -- SIPR
    CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN ACDcalls + voicemails ELSE 0 END) > 0 
         THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
         ELSE 0 END AS INT) as Cur_SiprASA,
    SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN callsoffered ELSE 0 END) as Cur_SiprOffered,
    SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN acdcalls + voicemails + callbacks ELSE 0 END) as Cur_SiprAnswered,

    -- NNPI
    CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN ACDcalls + voicemails ELSE 0 END) > 0 
         THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
         ELSE 0 END AS INT) as Cur_NnpiASA,
    SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN callsoffered ELSE 0 END) as Cur_NnpiOffered,
    SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN acdcalls + voicemails + callbacks ELSE 0 END) as Cur_NnpiAnswered,

    -- === MTD (Filtered by outer WHERE, no extra date filter needed) ===
    -- USN
    CAST(CASE WHEN SUM(CASE WHEN isreportingweb = 1 THEN ACDcalls + voicemails ELSE 0 END) > 0 
         THEN ROUND(SUM(CASE WHEN isreportingweb = 1 THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN isreportingweb = 1 THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
         ELSE 0 END AS INT) as Mtd_UsnASA,
    SUM(CASE WHEN isreportingweb = 1 THEN callsoffered ELSE 0 END) as Mtd_UsnOffered,
    SUM(CASE WHEN isreportingweb = 1 THEN acdcalls + voicemails + callbacks ELSE 0 END) as Mtd_UsnAnswered,

    -- VIP
    CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 46 THEN ACDcalls + voicemails ELSE 0 END) > 0 
         THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 46 THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 46 THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
         ELSE 0 END AS INT) as Mtd_VipASA,
    SUM(CASE WHEN CMS_Equivalent = 46 THEN callsoffered ELSE 0 END) as Mtd_VipOffered,
    SUM(CASE WHEN CMS_Equivalent = 46 THEN acdcalls + voicemails + callbacks ELSE 0 END) as Mtd_VipAnswered,

    -- SIPR
    CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 68 THEN ACDcalls + voicemails ELSE 0 END) > 0 
         THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 68 THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 68 THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
         ELSE 0 END AS INT) as Mtd_SiprASA,
    SUM(CASE WHEN CMS_Equivalent = 68 THEN callsoffered ELSE 0 END) as Mtd_SiprOffered,
    SUM(CASE WHEN CMS_Equivalent = 68 THEN acdcalls + voicemails + callbacks ELSE 0 END) as Mtd_SiprAnswered,

    -- NNPI
    CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 27 THEN ACDcalls + voicemails ELSE 0 END) > 0 
         THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 27 THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 27 THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
         ELSE 0 END AS INT) as Mtd_NnpiASA,
    SUM(CASE WHEN CMS_Equivalent = 27 THEN callsoffered ELSE 0 END) as Mtd_NnpiOffered,
    SUM(CASE WHEN CMS_Equivalent = 27 THEN acdcalls + voicemails + callbacks ELSE 0 END) as Mtd_NnpiAnswered

FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
WHERE CAST(edate AS date) >= @MonthStart
  AND CAST(edate AS date) <  @MonthEnd
  AND initiationmethod = 'INBOUND'
  AND (isreportingweb = 1 OR CMS_Equivalent IN (46, 68, 27))
OPTION (MAXDOP 1);

-- 2. Intervals (Current Day Only)
SELECT
    CONVERT(varchar(5), [interval], 108) + ' - ' +
    CONVERT(varchar(5), DATEADD(minute, 29, [interval]), 108) AS IntervalLabel,
    SUM(callsoffered)                                   AS CallsOffered,
    SUM(acdcalls + voicemails + callbacks)              AS Answered,
    CASE WHEN SUM(acdcalls + voicemails) > 0
         THEN CAST(SUM(CAST(anstime AS float)) / SUM(CAST(acdcalls + voicemails AS float)) AS decimal(10,2))
         ELSE 0
    END                                                 AS ASA
FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
WHERE initiationmethod = 'INBOUND'
  AND isreportingweb = 1
  AND CAST(edate AS date) >= @TodayStart
  AND CAST(edate AS date) <  @TodayEnd
GROUP BY [interval]
ORDER BY [interval]
OPTION (MAXDOP 1);";

            var p = new
            {
                TodayStart = todayStart.Date,
                TodayEnd = todayEnd.Date,
                MonthStart = monthStart.Date,
                MonthEnd = monthEnd.Date
            };

            var cmd = new CommandDefinition(sql, p, cancellationToken: ct, commandTimeout: 180);

            using var c = new SqlConnection(_connAws);
            await c.OpenAsync(ct);

            // Execute multiple queries in one round trip
            using var multi = await c.QueryMultipleAsync(cmd);

            var stats = await multi.ReadSingleOrDefaultAsync<CombinedStatsRow>() ?? new CombinedStatsRow();
            var intervals = (await multi.ReadAsync<IntervalGridRow>()).ToList();

            return (stats, intervals);
        }

        // ========================= Utilities =========================
        private static (DateTime start, DateTime end) GetMtdBounds(DateTime anchorEtLocal)
        {
            var first = new DateTime(anchorEtLocal.Year, anchorEtLocal.Month, 1);
            var start = first.Date;
            var end = start.AddMonths(1);
            return (start, end);
        }

        private async Task SafeStateHasChangedAsync()
        {
            if (_disposed) return;
            try { await InvokeAsync(StateHasChanged); }
            catch (Exception) { }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _cts.Cancel(); } catch { }
            _cts.Dispose();
        }

        // Helpers for Copy/Email
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
            catch (Exception ex)
            {
                Snackbar.Add($"Copy failed: {ex.Message}", Severity.Error);
            }
        }

        private async Task GenerateEmailAsync()
        {
            try
            {
                var auth = await AuthStateTask;
                var preparedBy = IdentityHelpers.GetSamAccount(auth.User);
                var ctx = BuildIntervalContextFromState();
                var (subject, html, to, cc, from) = await EmailSvc.ComposeAndLogAsync(ctx, _cts.Token);

                static string ToHHmm(object? t)
                {
                    if (t is DateTime dt) return dt.ToString("HHmm", CultureInfo.InvariantCulture);
                    if (t is TimeSpan ts) return ts.ToString(@"hhmm", CultureInfo.InvariantCulture);
                    return "0000";
                }
                static string ToHH_colon_mm(object? t)
                {
                    if (t is DateTime dt) return dt.ToString("HH':'mm", CultureInfo.InvariantCulture);
                    if (t is TimeSpan ts) return ts.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
                    return "00:00";
                }

                var startHHmm = ToHHmm(ctx.IntervalStart);
                var endHHmm = ToHHmm(ctx.IntervalEnd);
                var startHH_colon = ToHH_colon_mm(ctx.IntervalStart);
                var endHH_colon = ToHH_colon_mm(ctx.IntervalEnd);

                var subjectWithInterval = $"{subject} — {ctx.DateLocal:yyyy-MM-dd} {startHH_colon}-{endHH_colon} ET";

                var draft = new EmailDraft
                {
                    Subject = subjectWithInterval,
                    HtmlBody = html,
                    From = from?.Trim(),
                    To = to ?? string.Empty,
                    Cc = cc ?? string.Empty,
                    OpenAsDraft = true,
                    IncludeCui = true
                }.WithCuiBanner("Interval Summary", preparedBy, generatedEt: Et.Now);

                var bytes = EmailDraftBuilder.BuildMsgBytes(draft);
                var base64 = Convert.ToBase64String(bytes);
                var fileName = $"IntervalSummary_{ctx.DateLocal:yyyyMMdd}_{startHHmm}-{endHHmm}.oft";

                await JS.InvokeVoidAsync("downloadFileFromBase64", fileName, "application/vnd.ms-outlook", base64);
                Snackbar.Add("Draft generated — check your downloads.", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Could not generate email: {ex.Message}", Severity.Error);
            }
        }

        private IntervalEmailContext BuildIntervalContextFromState()
        {
            static int I(string? s) => int.TryParse(s, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var v) ? v : 0;
            static double F(string? s) => double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var v) ? v : 0d;
            static TimeSpan TS(string? s) => TimeSpan.TryParseExact(s?.Trim(), new[] { "h\\:mm", "hh\\:mm" }, CultureInfo.InvariantCulture, out var t) ? t : TimeSpan.Zero;

            var dateLocal = (State.Header.IntervalDate ?? Et.Now).Date;

            return new IntervalEmailContext
            {
                DateLocal = dateLocal,
                IntervalStart = TS(State.Header.IntervalStart),
                IntervalEnd = TS(State.Header.IntervalEnd),
                UsnAsa = F(State.Current.UsnAsa),
                UsnCallsOffered = I(State.Current.UsnCallsOffered),
                UsnCallsAnswered = I(State.Current.UsnCallsAnswered),
                VipAsa = F(State.Current.VipAsa),
                VipCallsOffered = I(State.Current.VipCallsOffered),
                VipCallsAnswered = I(State.Current.VipCallsAnswered),
                SiprAsa = F(State.Current.SiprAsa),
                SiprCallsOffered = I(State.Current.SiprCallsOffered),
                SiprCallsAnswered = I(State.Current.SiprCallsAnswered),
                NnpiAsa = F(State.Current.NnpiAsa),
                NnpiCallsOffered = I(State.Current.NnpiCallsOffered),
                NnpiCallsAnswered = I(State.Current.NnpiCallsAnswered),

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

                Slr33EmLos1 = F(State.Mtd.Slr33EmLos1),
                Slr33EmLos2 = F(State.Mtd.Slr33EmLos2),
                Slr33VmLos1 = F(State.Mtd.Slr33VmLos1),
                Slr33VmLos2 = F(State.Mtd.Slr33VmLos2),
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
                VmCount = I(State.Current.VmCount),
                VmOldestHours = F(State.Current.VmOldestHours),
                EssCount = I(State.Current.EssCount),
                EssOldestHours = F(State.Current.EssOldestHours),

                SrmAutoCount = I(State.Backlog.SrmAutoCount),
                SrmAutoAgeHours = F(State.Backlog.SrmAutoAgeHours),
                SrmUsnManCount = I(State.Backlog.SrmUsnManCount),
                SrmUsnManAgeHours = F(State.Backlog.SrmUsnManAgeHours),
                SrmSocManCount = I(State.Backlog.SrmSocManCount),
                SrmSocManAgeHours = F(State.Backlog.SrmSocManAgeHours),
                SrmValLineCount = I(State.Backlog.SrmValLineCount),
                SrmValLineAgeDays = F(State.Backlog.SrmValLineAgeDays),
                SrmValLineFailCount = I(State.Backlog.SrmValLineFailCount),
                SrmValLineFailAgeDays = F(State.Backlog.SrmValLineFailAgeDays),
                SrmValEmailCount = I(State.Backlog.SrmValEmailCount),
                SrmValEmailAgeDays = F(State.Backlog.SrmValEmailAgeDays),
                AfuCount = I(State.Backlog.AfuCount),
                AfuAgeHours = F(State.Backlog.AfuAgeHours),
                CsCount = I(State.Backlog.CsCount),
                CsAgeHours = F(State.Backlog.CsAgeHours),
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
                RdmUsnCount = I(State.Backlog.RdmUsnCount),
                RdmUsnAgeDays = F(State.Backlog.RdmUsnAgeDays),
                RdmEsdUsnCount = I(State.Backlog.RdmEsdUsnCount),
                RdmEsdUsnAgeDays = F(State.Backlog.RdmEsdUsnAgeDays),

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
    }
}