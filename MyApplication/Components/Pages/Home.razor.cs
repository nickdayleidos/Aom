using Dapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Tools;
using MediatR;
using MyApplication.Components.Features.HomeEvents.Queries;
using MyApplication.Components.Features.HomeEvents.Commands;
using MyApplication.Components.Service.Home;
using MyApplication.Components.Service;
using MyApplication.Components.Shared.Base;

namespace MyApplication.Components.Pages;

public partial class Home : AppComponentBase, IDisposable
{
    [Inject] private IConfiguration Config { get; set; } = default!;
    [Inject] private IDbContextFactory<AomDbContext> DbFactory { get; set; } = default!;
    [Inject] private IOiEventRepository OiRepo { get; set; } = default!;
    [Inject] private IMediator Mediator { get; set; } = default!;
    [Inject] private HomeDashboardCache Cache { get; set; } = default!;

    // ── Live data (fast queries, always fresh) ────────────────────
    private List<OiEvent> _openOi = [];
    private List<HomeEvent> _upcomingEvents = [];
    private bool _canEdit;

    // ── Local references into the singleton cache ─────────────────
    private IReadOnlyList<DashQueueRow> _dailyStats = [];
    private IReadOnlyList<DashQueueRow> _mtdStats = [];
    private IReadOnlyList<DashIntervalRow> _dailyIntervals = [];
    private IReadOnlyList<DashDateRow> _mtdByDate = [];
    private IReadOnlyList<DashSubTypeCount> _attendance = [];
    private IReadOnlyList<DashSubTypeCount> _resourcing = [];

    // ── KPI stat row computed properties ─────────────────────
    private int TotalCallsToday => _dailyStats.Sum(r => r.Offered);
    private int TotalAnsweredToday => _dailyStats.Sum(r => r.Answered);
    private string AnswerRateToday => TotalCallsToday == 0 ? "—"
        : $"{(TotalAnsweredToday * 100.0 / TotalCallsToday):F1}%";
    private string AvgAsaToday
    {
        get
        {
            var answered = _dailyStats.Sum(r => r.Answered);
            if (answered == 0) return "—";
            var weightedSum = _dailyStats.Sum(r => (long)r.Asa * r.Answered);
            return $"{weightedSum / answered:F0}s";
        }
    }
    private int OpenOiCount => _openOi.Count;
    private static Color AsaColor(int asa) => asa switch {
        0 => Color.Default,
        < 10 => Color.Success,
        < 20 => Color.Warning,
        _ => Color.Error
    };

    protected override async Task OnInitializedAsync()
    {
        // Subscribe so this component re-renders when any background refresh finishes
        Cache.RefreshCompleted += OnCacheRefreshed;

        // Snapshot cached data immediately — page renders without waiting
        ReadFromCache();

        // Load fast live data under the loading spinner
        await RunAsync(async () =>
        {
            var roles = await GetUserRolesAsync();
            _canEdit = roles.IsAdmin || roles.IsWfm || roles.IsOst;

            await Task.WhenAll(LoadOiAsync(), LoadUpcomingEventsAsync());
        });

        // Always refresh slow data in the background — cached data shows while it runs
        _ = RefreshSlowDataAsync();
    }

    private void ReadFromCache()
    {
        _dailyStats     = Cache.DailyStats;
        _mtdStats       = Cache.MtdStats;
        _dailyIntervals = Cache.DailyIntervals;
        _mtdByDate      = Cache.MtdByDate;
        _attendance     = Cache.Attendance;
        _resourcing     = Cache.Resourcing;
    }

    // ── Cache event handler ───────────────────────────────────────

    // Called on a thread-pool thread by HomeDashboardCache.EndRefresh().
    // Dispatch back to the Blazor circuit so the render is safe.
    private void OnCacheRefreshed() =>
        _ = InvokeAsync(() => { ReadFromCache(); StateHasChanged(); });

    public void Dispose() => Cache.RefreshCompleted -= OnCacheRefreshed;

    // ── Background refresh ────────────────────────────────────────

    private async Task RefreshSlowDataAsync()
    {
        if (!Cache.TryBeginRefresh()) return;

        try
        {
            await InvokeAsync(StateHasChanged); // show the refreshing indicator
            await Task.WhenAll(FetchAwsStatsAsync(), FetchScheduleDataAsync());
        }
        catch (Exception ex)
        {
            try { await InvokeAsync(() => ShowWarning($"Background refresh failed: {ex.Message}")); }
            catch { /* component may be disposed */ }
        }
        finally
        {
            Cache.EndRefresh(); // clears lock and fires RefreshCompleted → OnCacheRefreshed
        }
    }

    private async Task ManualRefreshAsync()
    {
        Cache.EndRefresh(); // clear any stale lock before triggering
        await RefreshSlowDataAsync();
    }

    // ── AWS call stats ────────────────────────────────────────────

    private async Task FetchAwsStatsAsync()
    {
        var connStr = Config.GetConnectionString("AWS") ?? "";
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        var todayEt = Et.Now.Date;
        var monthStart = new DateTime(todayEt.Year, todayEt.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        const string sql = @"
            -- 1. Aggregates (daily + MTD) by queue
            SELECT
                CAST(CASE WHEN SUM(CASE WHEN isreportingweb=1 AND CAST(edate AS date)>=@Today THEN acdcalls+voicemails ELSE 0 END)>0
                     THEN ROUND(SUM(CASE WHEN isreportingweb=1 AND CAST(edate AS date)>=@Today THEN CAST(anstime AS FLOAT) ELSE 0 END)
                              / SUM(CASE WHEN isreportingweb=1 AND CAST(edate AS date)>=@Today THEN CAST(acdcalls+voicemails AS FLOAT) ELSE 0 END),0)
                     ELSE 0 END AS INT) AS Cur_UsnAsa,
                SUM(CASE WHEN isreportingweb=1 AND CAST(edate AS date)>=@Today THEN callsoffered ELSE 0 END) AS Cur_UsnOffered,
                SUM(CASE WHEN isreportingweb=1 AND CAST(edate AS date)>=@Today THEN acdcalls+voicemails+callbacks ELSE 0 END) AS Cur_UsnAnswered,

                CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent=46 AND CAST(edate AS date)>=@Today THEN acdcalls+voicemails ELSE 0 END)>0
                     THEN ROUND(SUM(CASE WHEN CMS_Equivalent=46 AND CAST(edate AS date)>=@Today THEN CAST(anstime AS FLOAT) ELSE 0 END)
                              / SUM(CASE WHEN CMS_Equivalent=46 AND CAST(edate AS date)>=@Today THEN CAST(acdcalls+voicemails AS FLOAT) ELSE 0 END),0)
                     ELSE 0 END AS INT) AS Cur_VipAsa,
                SUM(CASE WHEN CMS_Equivalent=46 AND CAST(edate AS date)>=@Today THEN callsoffered ELSE 0 END) AS Cur_VipOffered,
                SUM(CASE WHEN CMS_Equivalent=46 AND CAST(edate AS date)>=@Today THEN acdcalls+voicemails+callbacks ELSE 0 END) AS Cur_VipAnswered,

                CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent=68 AND CAST(edate AS date)>=@Today THEN acdcalls+voicemails ELSE 0 END)>0
                     THEN ROUND(SUM(CASE WHEN CMS_Equivalent=68 AND CAST(edate AS date)>=@Today THEN CAST(anstime AS FLOAT) ELSE 0 END)
                              / SUM(CASE WHEN CMS_Equivalent=68 AND CAST(edate AS date)>=@Today THEN CAST(acdcalls+voicemails AS FLOAT) ELSE 0 END),0)
                     ELSE 0 END AS INT) AS Cur_SiprAsa,
                SUM(CASE WHEN CMS_Equivalent=68 AND CAST(edate AS date)>=@Today THEN callsoffered ELSE 0 END) AS Cur_SiprOffered,
                SUM(CASE WHEN CMS_Equivalent=68 AND CAST(edate AS date)>=@Today THEN acdcalls+voicemails+callbacks ELSE 0 END) AS Cur_SiprAnswered,

                CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent=27 AND CAST(edate AS date)>=@Today THEN acdcalls+voicemails ELSE 0 END)>0
                     THEN ROUND(SUM(CASE WHEN CMS_Equivalent=27 AND CAST(edate AS date)>=@Today THEN CAST(anstime AS FLOAT) ELSE 0 END)
                              / SUM(CASE WHEN CMS_Equivalent=27 AND CAST(edate AS date)>=@Today THEN CAST(acdcalls+voicemails AS FLOAT) ELSE 0 END),0)
                     ELSE 0 END AS INT) AS Cur_NnpiAsa,
                SUM(CASE WHEN CMS_Equivalent=27 AND CAST(edate AS date)>=@Today THEN callsoffered ELSE 0 END) AS Cur_NnpiOffered,
                SUM(CASE WHEN CMS_Equivalent=27 AND CAST(edate AS date)>=@Today THEN acdcalls+voicemails+callbacks ELSE 0 END) AS Cur_NnpiAnswered,

                -- MTD
                CAST(CASE WHEN SUM(CASE WHEN isreportingweb=1 THEN acdcalls+voicemails ELSE 0 END)>0
                     THEN ROUND(SUM(CASE WHEN isreportingweb=1 THEN CAST(anstime AS FLOAT) ELSE 0 END)
                              / SUM(CASE WHEN isreportingweb=1 THEN CAST(acdcalls+voicemails AS FLOAT) ELSE 0 END),0)
                     ELSE 0 END AS INT) AS Mtd_UsnAsa,
                SUM(CASE WHEN isreportingweb=1 THEN callsoffered ELSE 0 END) AS Mtd_UsnOffered,
                SUM(CASE WHEN isreportingweb=1 THEN acdcalls+voicemails+callbacks ELSE 0 END) AS Mtd_UsnAnswered,

                CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent=46 THEN acdcalls+voicemails ELSE 0 END)>0
                     THEN ROUND(SUM(CASE WHEN CMS_Equivalent=46 THEN CAST(anstime AS FLOAT) ELSE 0 END)
                              / SUM(CASE WHEN CMS_Equivalent=46 THEN CAST(acdcalls+voicemails AS FLOAT) ELSE 0 END),0)
                     ELSE 0 END AS INT) AS Mtd_VipAsa,
                SUM(CASE WHEN CMS_Equivalent=46 THEN callsoffered ELSE 0 END) AS Mtd_VipOffered,
                SUM(CASE WHEN CMS_Equivalent=46 THEN acdcalls+voicemails+callbacks ELSE 0 END) AS Mtd_VipAnswered,

                CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent=68 THEN acdcalls+voicemails ELSE 0 END)>0
                     THEN ROUND(SUM(CASE WHEN CMS_Equivalent=68 THEN CAST(anstime AS FLOAT) ELSE 0 END)
                              / SUM(CASE WHEN CMS_Equivalent=68 THEN CAST(acdcalls+voicemails AS FLOAT) ELSE 0 END),0)
                     ELSE 0 END AS INT) AS Mtd_SiprAsa,
                SUM(CASE WHEN CMS_Equivalent=68 THEN callsoffered ELSE 0 END) AS Mtd_SiprOffered,
                SUM(CASE WHEN CMS_Equivalent=68 THEN acdcalls+voicemails+callbacks ELSE 0 END) AS Mtd_SiprAnswered,

                CAST(CASE WHEN SUM(CASE WHEN CMS_Equivalent=27 THEN acdcalls+voicemails ELSE 0 END)>0
                     THEN ROUND(SUM(CASE WHEN CMS_Equivalent=27 THEN CAST(anstime AS FLOAT) ELSE 0 END)
                              / SUM(CASE WHEN CMS_Equivalent=27 THEN CAST(acdcalls+voicemails AS FLOAT) ELSE 0 END),0)
                     ELSE 0 END AS INT) AS Mtd_NnpiAsa,
                SUM(CASE WHEN CMS_Equivalent=27 THEN callsoffered ELSE 0 END) AS Mtd_NnpiOffered,
                SUM(CASE WHEN CMS_Equivalent=27 THEN acdcalls+voicemails+callbacks ELSE 0 END) AS Mtd_NnpiAnswered

            FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
            WHERE CAST(edate AS date) >= @MonthStart
              AND CAST(edate AS date) <  @MonthEnd
              AND initiationmethod = 'INBOUND'
              AND (isreportingweb=1 OR CMS_Equivalent IN (46,68,27))
            OPTION (MAXDOP 1);

            -- 2. Today by interval (combined)
            SELECT
                CONVERT(varchar(5),[interval],108)+' - '+CONVERT(varchar(5),DATEADD(minute,29,[interval]),108) AS Label,
                SUM(callsoffered) AS Offered,
                SUM(acdcalls+voicemails+callbacks) AS Answered,
                CASE WHEN SUM(acdcalls+voicemails)>0
                     THEN CAST(SUM(anstime)/SUM(acdcalls+voicemails) AS INT) ELSE 0 END AS Asa
            FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
            WHERE CAST(edate AS date)=@Today
              AND initiationmethod='INBOUND'
              AND (isreportingweb=1 OR CMS_Equivalent IN (46,68,27))
            GROUP BY [interval]
            ORDER BY [interval];

            -- 3. MTD by date (combined)
            SELECT
                CAST(edate AS date) AS TheDate,
                SUM(callsoffered) AS Offered,
                SUM(acdcalls+voicemails+callbacks) AS Answered,
                CASE WHEN SUM(acdcalls+voicemails)>0
                     THEN CAST(SUM(anstime)/SUM(acdcalls+voicemails) AS INT) ELSE 0 END AS Asa
            FROM todnmciaws.dbo.hsplitdata WITH (NOLOCK)
            WHERE CAST(edate AS date)>=@MonthStart
              AND CAST(edate AS date)<@MonthEnd
              AND initiationmethod='INBOUND'
              AND (isreportingweb=1 OR CMS_Equivalent IN (46,68,27))
            GROUP BY CAST(edate AS date)
            ORDER BY CAST(edate AS date);";

        using var multi = await conn.QueryMultipleAsync(sql, new
        {
            Today = todayEt,
            MonthStart = monthStart,
            MonthEnd = monthEnd
        });

        var agg = await multi.ReadFirstOrDefaultAsync();
        List<DashQueueRow> daily = [], mtd = [];
        if (agg != null)
        {
            daily =
            [
                new("USN",  (int)(agg.Cur_UsnOffered  ?? 0), (int)(agg.Cur_UsnAnswered  ?? 0), (int)(agg.Cur_UsnAsa  ?? 0)),
                new("VIP",  (int)(agg.Cur_VipOffered  ?? 0), (int)(agg.Cur_VipAnswered  ?? 0), (int)(agg.Cur_VipAsa  ?? 0)),
                new("SIPR", (int)(agg.Cur_SiprOffered ?? 0), (int)(agg.Cur_SiprAnswered ?? 0), (int)(agg.Cur_SiprAsa ?? 0)),
                new("NNPI", (int)(agg.Cur_NnpiOffered ?? 0), (int)(agg.Cur_NnpiAnswered ?? 0), (int)(agg.Cur_NnpiAsa ?? 0)),
            ];
            mtd =
            [
                new("USN",  (int)(agg.Mtd_UsnOffered  ?? 0), (int)(agg.Mtd_UsnAnswered  ?? 0), (int)(agg.Mtd_UsnAsa  ?? 0)),
                new("VIP",  (int)(agg.Mtd_VipOffered  ?? 0), (int)(agg.Mtd_VipAnswered  ?? 0), (int)(agg.Mtd_VipAsa  ?? 0)),
                new("SIPR", (int)(agg.Mtd_SiprOffered ?? 0), (int)(agg.Mtd_SiprAnswered ?? 0), (int)(agg.Mtd_SiprAsa ?? 0)),
                new("NNPI", (int)(agg.Mtd_NnpiOffered ?? 0), (int)(agg.Mtd_NnpiAnswered ?? 0), (int)(agg.Mtd_NnpiAsa ?? 0)),
            ];
        }

        var intervalData = await multi.ReadAsync<dynamic>();
        var intervals = intervalData.Select(x => new DashIntervalRow(
            (string)x.Label, (int)x.Offered, (int)x.Answered, (int)x.Asa
        )).ToList();

        var dateData = await multi.ReadAsync<dynamic>();
        var byDate = dateData.Select(x => new DashDateRow(
            DateOnly.FromDateTime((DateTime)x.TheDate), (int)x.Offered, (int)x.Answered, (int)x.Asa
        )).ToList();

        Cache.Update(daily, mtd, intervals, byDate, Cache.Attendance.ToList(), Cache.Resourcing.ToList());
    }

    // ── Schedule data ─────────────────────────────────────────────

    private async Task FetchScheduleDataAsync()
    {
        var today = DateOnly.FromDateTime(Et.Now.Date);
        await using var db = await DbFactory.CreateDbContextAsync();

        var rows = await db.DetailedSchedule
            .AsNoTracking()
            .Include(s => s.ActivitySubType)
            .Where(s => s.ScheduleDate == today && (s.ScheduleTypeId == 5 || s.ScheduleTypeId == 6))
            .Select(s => new { s.ScheduleTypeId, s.EmployeeId, SubTypeName = s.ActivitySubType!.Name })
            .ToListAsync();

        var attendance = rows
            .Where(r => r.ScheduleTypeId == 5)
            .GroupBy(r => r.SubTypeName)
            .Select(g => new DashSubTypeCount(g.Key, g.Select(x => x.EmployeeId).Distinct().Count()))
            .OrderBy(x => x.SubTypeName)
            .ToList();

        var resourcing = rows
            .Where(r => r.ScheduleTypeId == 6)
            .GroupBy(r => r.SubTypeName)
            .Select(g => new DashSubTypeCount(g.Key, g.Select(x => x.EmployeeId).Distinct().Count()))
            .OrderBy(x => x.SubTypeName)
            .ToList();

        Cache.Update(
            Cache.DailyStats.ToList(), Cache.MtdStats.ToList(),
            Cache.DailyIntervals.ToList(), Cache.MtdByDate.ToList(),
            attendance, resourcing);
    }

    // ── Live queries ──────────────────────────────────────────────

    private async Task LoadOiAsync()
    {
        var monthAgo = DateTime.UtcNow.AddDays(-90);
        var items = await OiRepo.ListAsync(monthAgo, null);
        _openOi = items.Where(e => e.ResolutionTime == null).OrderByDescending(e => e.PostedTime).ToList();
    }

    private async Task LoadUpcomingEventsAsync()
    {
        _upcomingEvents = (await Mediator.Send(new ListUpcomingEventsQuery())).ToList();
    }

    // ── Upcoming Events CRUD ──────────────────────────────────────

    private async Task OpenAddEventDialogAsync()
    {
        var evt = new HomeEvent { EventDate = DateTime.Today };
        var parameters = new DialogParameters<HomeEventDialog> { { x => x.Event, evt }, { x => x.IsNew, true } };
        var dialog = await DialogService.ShowAsync<HomeEventDialog>("Add Event", parameters,
            new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true });
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: HomeEvent saved })
        {
            var auth = await AuthStateProvider.GetAuthenticationStateAsync();
            saved.CreatedBy = auth.User.Identity?.Name ?? "";
            saved.CreatedAt = DateTime.UtcNow;
            await Mediator.Send(new CreateHomeEventCommand(saved));
            await LoadUpcomingEventsAsync();
            StateHasChanged();
        }
    }

    private async Task OpenEditEventDialogAsync(HomeEvent evt)
    {
        var copy = new HomeEvent { Id = evt.Id, Title = evt.Title, EventDate = evt.EventDate, Description = evt.Description, CreatedBy = evt.CreatedBy, CreatedAt = evt.CreatedAt };
        var parameters = new DialogParameters<HomeEventDialog> { { x => x.Event, copy }, { x => x.IsNew, false } };
        var dialog = await DialogService.ShowAsync<HomeEventDialog>("Edit Event", parameters,
            new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true });
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: HomeEvent saved })
        {
            await Mediator.Send(new UpdateHomeEventCommand(saved));
            await LoadUpcomingEventsAsync();
            StateHasChanged();
        }
    }

    private async Task DeleteEventAsync(HomeEvent evt)
    {
        if (!await ConfirmAsync("Delete Event", $"Delete \"{evt.Title}\"?")) return;
        await Mediator.Send(new DeleteHomeEventCommand(evt.Id));
        await LoadUpcomingEventsAsync();
        StateHasChanged();
    }
}
