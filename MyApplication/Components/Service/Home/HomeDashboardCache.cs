namespace MyApplication.Components.Service.Home;

/// <summary>
/// Singleton in-memory cache for the slow AWS + schedule queries on the home dashboard.
/// The component reads from this immediately (fast render) and triggers a background
/// refresh when data is stale or missing.
/// </summary>
public sealed class HomeDashboardCache
{
    // 0 = idle, 1 = refreshing  (Interlocked requires int)
    private int _refreshing;

    public bool IsRefreshing => _refreshing == 1;
    public DateTime? LastRefreshed { get; private set; }
    public bool HasData => LastRefreshed.HasValue;

    // ── Call Performance ──────────────────────────────────────────
    public IReadOnlyList<DashQueueRow> DailyStats { get; private set; } = [];
    public IReadOnlyList<DashQueueRow> MtdStats { get; private set; } = [];
    public IReadOnlyList<DashIntervalRow> DailyIntervals { get; private set; } = [];
    public IReadOnlyList<DashDateRow> MtdByDate { get; private set; } = [];

    // ── Schedule ──────────────────────────────────────────────────
    public IReadOnlyList<DashSubTypeCount> Attendance { get; private set; } = [];
    public IReadOnlyList<DashSubTypeCount> Resourcing { get; private set; } = [];

    /// <summary>Returns true if the refresh lock was acquired (no refresh already running).</summary>
    public bool TryBeginRefresh() =>
        System.Threading.Interlocked.CompareExchange(ref _refreshing, 1, 0) == 0;

    public void Update(
        List<DashQueueRow> daily, List<DashQueueRow> mtd,
        List<DashIntervalRow> intervals, List<DashDateRow> byDate,
        List<DashSubTypeCount> attendance, List<DashSubTypeCount> resourcing)
    {
        DailyStats = daily;
        MtdStats = mtd;
        DailyIntervals = intervals;
        MtdByDate = byDate;
        Attendance = attendance;
        Resourcing = resourcing;
        LastRefreshed = DateTime.Now;
    }

    /// <summary>
    /// Fired on the thread-pool thread when a refresh completes (success or failure).
    /// Subscribers must dispatch to their own synchronization context if needed.
    /// </summary>
    public event Action? RefreshCompleted;

    public void EndRefresh()
    {
        System.Threading.Interlocked.Exchange(ref _refreshing, 0);
        RefreshCompleted?.Invoke();
    }
}

public record DashQueueRow(string Queue, int Offered, int Answered, int Asa);
public record DashIntervalRow(string Label, int Offered, int Answered, int Asa);
public record DashDateRow(DateOnly Date, int Offered, int Answered, int Asa);
public record DashSubTypeCount(string SubTypeName, int Count);
