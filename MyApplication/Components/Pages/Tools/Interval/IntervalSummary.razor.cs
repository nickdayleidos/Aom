using Dapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.JSInterop;
using MudBlazor;
using MyApplication.Common;
using MyApplication.Common.Time;
using MyApplication.Components.Service;
using MyApplication.Components.Services.Email;
using System.Data;
using System.Globalization;
using System.Text;

namespace MyApplication.Components.Pages.Tools.Interval
{
    public partial class IntervalSummary : ComponentBase, IDisposable
    {
        [Inject] private IConfiguration Config { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IEmailComposer EmailComposer { get; set; } = default!;
        [Inject] private IntervalEmailService EmailSvc { get; set; } = default!;
        [Inject] private IIntervalSummaryRepository Repo { get; set; } = default!;
        [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = default!;

        private IntervalSummaryState State { get; set; } = new();

        private string _connAws = string.Empty;
        private readonly CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            _connAws = Config.GetConnectionString("AWS") ?? "";

            // 2. Populate Notes from latest entry in Database

            await PopulateAsync();
        }

        private async Task PopulateAsync()
        {
            // 1. Force Reset Time to Previous Hour
            var etNowMinus30 = Et.Now.AddMinutes(-30);
            var startHour = (etNowMinus30.Hour / 4) * 4;
            var endHour = startHour + 4;

            State.Header.IntervalDate = etNowMinus30.Date;
            State.Header.IntervalStart = $"{startHour:00}:00";
            State.Header.IntervalEnd = $"{endHour:00}:00";
            State.Header.PopulateTime = TimeZoneInfo.ConvertTimeFromUtc(
                                                DateTime.UtcNow,
                                                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));


            // 2. Explicitly Re-load Notes (using new property names)
            await LoadLatestNotesAsync();

            // 3. Fetch AWS Data
            try
            {
                using var conn = new SqlConnection(_connAws);
                await conn.OpenAsync();

                var todayStart = State.Header.IntervalDate?.Date ?? DateTime.Today;
                var monthStart = new DateTime(todayStart.Year, todayStart.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                // SQL Query (Aggregates + Intervals)
                var sql = @"
                    -- 1. Aggregates (Current + MTD)
                    SELECT 
                        -- === CURRENT DAY (Filtered by @TodayStart) ===
                        CAST(CASE WHEN SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN ACDcalls + voicemails ELSE 0 END) > 0 
                             THEN ROUND(SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
                             ELSE 0 END AS INT) as Cur_UsnASA,
                        SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN callsoffered ELSE 0 END) as Cur_UsnOffered,
                        SUM(CASE WHEN isreportingweb = 1 AND CAST(edate AS date) >= @TodayStart THEN acdcalls + voicemails + callbacks ELSE 0 END) as Cur_UsnAnswered,

                        CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN ACDcalls + voicemails ELSE 0 END) > 0 
                             THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
                             ELSE 0 END AS INT) as Cur_VipASA,
                        SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN callsoffered ELSE 0 END) as Cur_VipOffered,
                        SUM(CASE WHEN CMS_Equivalent = 46 AND CAST(edate AS date) >= @TodayStart THEN acdcalls + voicemails + callbacks ELSE 0 END) as Cur_VipAnswered,

                        CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN ACDcalls + voicemails ELSE 0 END) > 0 
                             THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
                             ELSE 0 END AS INT) as Cur_SiprASA,
                        SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN callsoffered ELSE 0 END) as Cur_SiprOffered,
                        SUM(CASE WHEN CMS_Equivalent = 68 AND CAST(edate AS date) >= @TodayStart THEN acdcalls + voicemails + callbacks ELSE 0 END) as Cur_SiprAnswered,

                        CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN ACDcalls + voicemails ELSE 0 END) > 0 
                             THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
                             ELSE 0 END AS INT) as Cur_NnpiASA,
                        SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN callsoffered ELSE 0 END) as Cur_NnpiOffered,
                        SUM(CASE WHEN CMS_Equivalent = 27 AND CAST(edate AS date) >= @TodayStart THEN acdcalls + voicemails + callbacks ELSE 0 END) as Cur_NnpiAnswered,

                        -- === MTD (Filtered by @MonthStart / @MonthEnd) ===
                        CAST(CASE WHEN SUM(CASE WHEN isreportingweb = 1 THEN ACDcalls + voicemails ELSE 0 END) > 0 
                             THEN ROUND(SUM(CASE WHEN isreportingweb = 1 THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN isreportingweb = 1 THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
                             ELSE 0 END AS INT) as Mtd_UsnASA,
                        SUM(CASE WHEN isreportingweb = 1 THEN callsoffered ELSE 0 END) as Mtd_UsnOffered,
                        SUM(CASE WHEN isreportingweb = 1 THEN acdcalls + voicemails + callbacks ELSE 0 END) as Mtd_UsnAnswered,

                        CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 46 THEN ACDcalls + voicemails ELSE 0 END) > 0 
                             THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 46 THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 46 THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
                             ELSE 0 END AS INT) as Mtd_VipASA,
                        SUM(CASE WHEN CMS_Equivalent = 46 THEN callsoffered ELSE 0 END) as Mtd_VipOffered,
                        SUM(CASE WHEN CMS_Equivalent = 46 THEN acdcalls + voicemails + callbacks ELSE 0 END) as Mtd_VipAnswered,

                        CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent = 68 THEN ACDcalls + voicemails ELSE 0 END) > 0 
                             THEN ROUND(SUM(CASE WHEN CMS_Equivalent = 68 THEN CAST(anstime AS FLOAT) ELSE 0 END) / SUM(CASE WHEN CMS_Equivalent = 68 THEN CAST(ACDcalls + voicemails AS FLOAT) ELSE 0 END), 0)
                             ELSE 0 END AS INT) as Mtd_SiprASA,
                        SUM(CASE WHEN CMS_Equivalent = 68 THEN callsoffered ELSE 0 END) as Mtd_SiprOffered,
                        SUM(CASE WHEN CMS_Equivalent = 68 THEN acdcalls + voicemails + callbacks ELSE 0 END) as Mtd_SiprAnswered,

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
                             THEN SUM(anstime) / SUM(acdcalls + voicemails)
                             ELSE 0
                        END AS ASA
                    FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
                    WHERE CAST(edate AS date) = @TodayStart
                      AND initiationmethod = 'INBOUND'
                      AND (isreportingweb = 1 OR CMS_Equivalent IN (46, 68, 27))
                    GROUP BY [interval]
                    ORDER BY [interval];
                ";

                using var multi = await conn.QueryMultipleAsync(sql, new { TodayStart = todayStart, MonthStart = monthStart, MonthEnd = monthEnd });

                // --- 1. Map Aggregates ---
                var result = await multi.ReadFirstOrDefaultAsync();
                if (result != null)
                {
                    // Current Day
                    State.Current.UsnCallsOffered = result.Cur_UsnOffered?.ToString() ?? "0";
                    State.Current.UsnCallsAnswered = result.Cur_UsnAnswered?.ToString() ?? "0";
                    State.Current.UsnAsa = result.Cur_UsnASA?.ToString() ?? "0";

                    State.Current.VipCallsOffered = result.Cur_VipOffered?.ToString() ?? "0";
                    State.Current.VipCallsAnswered = result.Cur_VipAnswered?.ToString() ?? "0";
                    State.Current.VipAsa = result.Cur_VipASA?.ToString() ?? "0";

                    State.Current.SiprCallsOffered = result.Cur_SiprOffered?.ToString() ?? "0";
                    State.Current.SiprCallsAnswered = result.Cur_SiprAnswered?.ToString() ?? "0";
                    State.Current.SiprAsa = result.Cur_SiprASA?.ToString() ?? "0";

                    State.Current.NnpiCallsOffered = result.Cur_NnpiOffered?.ToString() ?? "0";
                    State.Current.NnpiCallsAnswered = result.Cur_NnpiAnswered?.ToString() ?? "0";
                    State.Current.NnpiAsa = result.Cur_NnpiASA?.ToString() ?? "0";

                    // MTD
                    State.Mtd.UsnCallsOffered = result.Mtd_UsnOffered?.ToString() ?? "0";
                    State.Mtd.UsnCallsAnswered = result.Mtd_UsnAnswered?.ToString() ?? "0";
                    State.Mtd.UsnAsa = result.Mtd_UsnASA?.ToString() ?? "0";

                    State.Mtd.VipCallsOffered = result.Mtd_VipOffered?.ToString() ?? "0";
                    State.Mtd.VipCallsAnswered = result.Mtd_VipAnswered?.ToString() ?? "0";
                    State.Mtd.VipAsa = result.Mtd_VipASA?.ToString() ?? "0";

                    State.Mtd.SiprCallsOffered = result.Mtd_SiprOffered?.ToString() ?? "0";
                    State.Mtd.SiprCallsAnswered = result.Mtd_SiprAnswered?.ToString() ?? "0";
                    State.Mtd.SiprAsa = result.Mtd_SiprASA?.ToString() ?? "0";

                    State.Mtd.NnpiCallsOffered = result.Mtd_NnpiOffered?.ToString() ?? "0";
                    State.Mtd.NnpiCallsAnswered = result.Mtd_NnpiAnswered?.ToString() ?? "0";
                    State.Mtd.NnpiAsa = result.Mtd_NnpiASA?.ToString() ?? "0";
                }

                // --- 2. Map Intervals Grid ---
                var intervalData = await multi.ReadAsync<dynamic>();
                State.Intervals = intervalData.Select(x => new IntervalSummaryState.IntervalData
                {
                    IntervalLabel = x.IntervalLabel,
                    CallsOffered = (int)x.CallsOffered,
                    Answered = (int)x.Answered,
                    Asa = (double)(decimal)x.ASA
                }).ToList();

                Snackbar.Add("Interval times and stats populated.", Severity.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AWS Fetch Error: {ex.Message}");
                Snackbar.Add($"Error fetching AWS stats: {ex.Message}. Time updated.", Severity.Warning);
            }
        }

        private async Task LoadLatestNotesAsync()
        {
            try
            {
                var latest = await Repo.GetLatestAsync();

                if (latest != null)
                {
                    // UPDATED: Using new property names
                    State.Notes.NaTodaysFocusArea = latest.NaTodaysFocusArea ?? "";
                    State.Notes.NaMajorCirImpact = latest.NaMajorCirImpact ?? "";
                    State.Notes.NaImpactingEvents = latest.NaImpactingEvents ?? "";
                    State.Notes.NaHpsmStatus = latest.NaHpsmStatus ?? "";
                    State.Notes.NaManagementNotes = latest.NaManagementNotes ?? "";

                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching latest summary: {ex.Message}");
                Snackbar.Add($"Error loading notes: {ex.Message}", Severity.Warning);
            }
        }

        private void SetPreviousHourInterval()
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(-1);
            var end = start.AddHours(1);

            State.Header.IntervalDate = now.Date;
            State.Header.IntervalStart = start.ToString("HH:mm");
            State.Header.IntervalEnd = end.ToString("HH:mm");
        }

        private async Task CopyStatsAsync()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Interval: {State.Header.IntervalStart} - {State.Header.IntervalEnd}");
            sb.AppendLine("--- Current ---");
            sb.AppendLine($"USN: {State.Current.UsnCallsOffered}/{State.Current.UsnCallsAnswered} (ASA: {State.Current.UsnAsa})");
            sb.AppendLine($"VIP: {State.Current.VipCallsOffered}/{State.Current.VipCallsAnswered} (ASA: {State.Current.VipAsa})");
            sb.AppendLine($"SIPR: {State.Current.SiprCallsOffered}/{State.Current.SiprCallsAnswered} (ASA: {State.Current.SiprAsa})");

            await JS.InvokeVoidAsync("navigator.clipboard.writeText", sb.ToString());
            Snackbar.Add("Stats copied to clipboard.", Severity.Success);
        }

        private async Task ClearAsync()
        {
            State = new IntervalSummaryState();
            SetPreviousHourInterval();
            await Task.CompletedTask;
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
                DateLocal = State.Header.IntervalDate ?? DateTime.Today,
                IntervalStart = TimeSpan.TryParse(State.Header.IntervalStart, out var tsStart) ? tsStart : TimeSpan.Zero,
                IntervalEnd = TimeSpan.TryParse(State.Header.IntervalEnd, out var tsEnd) ? tsEnd : TimeSpan.Zero,

                // ... (Stats mapping logic remains the same)
                UsnCallsOffered = I(State.Current.UsnCallsOffered),
                UsnCallsAnswered = I(State.Current.UsnCallsAnswered),
                UsnAsa = F(State.Current.UsnAsa),
                VipCallsOffered = I(State.Current.VipCallsOffered),
                VipCallsAnswered = I(State.Current.VipCallsAnswered),
                VipAsa = F(State.Current.VipAsa),
                SiprCallsOffered = I(State.Current.SiprCallsOffered),
                SiprCallsAnswered = I(State.Current.SiprCallsAnswered),
                SiprAsa = F(State.Current.SiprAsa),
                NnpiCallsOffered = I(State.Current.NnpiCallsOffered),
                NnpiCallsAnswered = I(State.Current.NnpiCallsAnswered),
                NnpiAsa = F(State.Current.NnpiAsa),

                CurrentEmailCount = I(State.Current.CurrentEmailCount),
                CurrentEmailOldest = F(State.Current.CurrentEmailAgeHours),
                CurrentCustomerCareCount = I(State.Current.CurrentCustomerCareCount),
                CurrentCustomerCareOldest = F(State.Current.CurrentCustomerCareAgeHours),

                CurrentSiprEmailCount = I(State.Current.CurrentSiprEmailCount),
                CurrentSiprEmailOldest = F(State.Current.CurrentSiprEmailAgeHours),
                CurrentSiprGdaSpreadsheets = I(State.Current.CurrentSiprGdaSpreadsheets),
                CurrentSiprGdaOldest = F(State.Current.CurrentSiprGdaAgeHours),
                CurrentSiprUaifCount = I(State.Current.CurrentSiprUaifCount),
                CurrentSiprUaifOldest = F(State.Current.CurrentSiprUaifAgeHours),

                CurrentVmCount = I(State.Current.CurrentVmCount),
                CurrentVmOldest = F(State.Current.CurrentVmAgeHours),
                CurrentEssCount = I(State.Current.CurrentEssCount),
                CurrentEssOldest = F(State.Current.CurrentEssAgeHours),

                MtdUsnCallsOffered = I(State.Mtd.UsnCallsOffered),
                MtdUsnCallsAnswered = I(State.Mtd.UsnCallsAnswered),
                MtdUsnAsa = F(State.Mtd.UsnAsa),
                MtdVipCallsOffered = I(State.Mtd.VipCallsOffered),
                MtdVipCallsAnswered = I(State.Mtd.VipCallsAnswered),
                MtdVipAsa = F(State.Mtd.VipAsa),
                MtdSiprCallsOffered = I(State.Mtd.SiprCallsOffered),
                MtdSiprCallsAnswered = I(State.Mtd.SiprCallsAnswered),
                MtdSiprAsa = F(State.Mtd.SiprAsa),
                MtdNnpiCallsOffered = I(State.Mtd.NnpiCallsOffered),
                MtdNnpiCallsAnswered = I(State.Mtd.NnpiCallsAnswered),
                MtdNnpiAsa = F(State.Mtd.NnpiAsa),

                Slr33EmMtdLos1 = F(State.Mtd.Slr33EmLos1),
                Slr33EmMtdLos2 = F(State.Mtd.Slr33EmLos2),
                Slr33VmMtdLos1 = F(State.Mtd.Slr33VmLos1),
                Slr33VmMtdLos2 = F(State.Mtd.Slr33VmLos2),



                AfuCount = I(State.Backlog.AfuCount),
                AfuAgeHours = F(State.Backlog.AfuAgeDays),
                CsCount = I(State.Backlog.CsCount),
                CsAgeHours = F(State.Backlog.CsAgeDays),

                SrmUaAutoCount = I(State.Backlog.SrmUaAutoCount),
                SrmUaAutoAgeHours = F(State.Backlog.SrmUaAutoAgeDays),
                SrmUaUsnManCount = I(State.Backlog.SrmUaUsnManCount),
                SrmUaUsnManAgeHours = F(State.Backlog.SrmUaUsnManAgeDays),
                SrmUaSocManCount = I(State.Backlog.SrmUaSocManCount),
                SrmUaSocManAgeHours = F(State.Backlog.SrmUaSocManAgeDays),

                SrmValCount = I(State.Backlog.SrmValCount),
                SrmValAgeDays = F(State.Backlog.SrmValAgeDays),
                SrmValLineFailCount = I(State.Backlog.SrmValLineFailCount),
                SrmValLineFailAgeDays = F(State.Backlog.SrmValLineFailAgeDays),
                SrmValEmailCount = I(State.Backlog.SrmValEmailCount),
                SrmValEmailAgeDays = F(State.Backlog.SrmValEmailAgeDays),

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

                NnpiQueue = I(State.Backlog.NnpiQueue),
                SiprQueue = I(State.Backlog.SiprQueue),
                NcisQueue = I(State.Backlog.NcisQueue),
                VipQueue = I(State.Backlog.VipQueue),
                RdmNnpiQueue = I(State.Backlog.RdmNnpiQueue),
                RdmSiprQueue = I(State.Backlog.RdmSiprQueue),

                // UPDATED: Use new properties
                NaTodaysFocusArea = State.Notes.NaTodaysFocusArea,
                NaMajorCirImpact = State.Notes.NaMajorCirImpact,
                NaImpactingEvents = State.Notes.NaImpactingEvents,
                NaHpsmStatus = State.Notes.NaHpsmStatus,
                NaManagementNotes = State.Notes.NaManagementNotes
            };
        }


        // Helpers
        private int I(string s) => int.TryParse(s, out var v) ? v : 0;
        private double F(string s) => double.TryParse(s, out var v) ? v : 0.0;

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}