using Dapper;
using Microsoft.Data.SqlClient;
using MyApplication.Components.Model.AOM.Employee; // Site
using MyApplication.Components.Model.AOM.Tools;    // OI models

namespace MyApplication.Components.Service
{
    public interface IOiLookupRepository
    {
        Task<IReadOnlyList<OiCategory>> GetCategoriesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<OiSeverity>> GetSeveritiesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Site>> GetSitesAsync(CancellationToken ct = default);
        Task<OiSeverity?> GetSeverityAsync(int id, CancellationToken ct = default);
    }

    public interface IOiEventRepository
    {
        Task<int> InsertAsync(OiEvent e, CancellationToken ct = default);                       // <-- keep ONE
        Task UpdateAsync(OiEvent e, CancellationToken ct = default);
        Task<OiEvent?> GetAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<OiEvent>> ListAsync(DateTime? fromLocal, DateTime? toLocal, CancellationToken ct = default);

        Task<int> InsertUpdateAsync(OiEventUpdate u, bool allClear = false, CancellationToken ct = default);

        Task<IReadOnlyList<OiEventUpdate>> ListUpdatesAsync(int eventId, CancellationToken ct = default);
    }

    public sealed class OiLookupRepository : IOiLookupRepository
    {
        private readonly IDbConnectionFactory _factory;
        public OiLookupRepository(IDbConnectionFactory f) => _factory = f;

        public async Task<IReadOnlyList<OiCategory>> GetCategoriesAsync(CancellationToken ct = default)
        {
            const string sql = "SELECT Id, Name, IsActive FROM [Tools].[OiCategory] WHERE IsActive = 1 ORDER BY Name;";
            await using var cn = _factory.Create("AOM");
            var rows = await cn.QueryAsync<OiCategory>(new CommandDefinition(sql, cancellationToken: ct));
            return rows.AsList();
        }

        public async Task<IReadOnlyList<OiSeverity>> GetSeveritiesAsync(CancellationToken ct = default)
        {
            const string sql = @"
SELECT Id, Name, IsActive
FROM [Tools].[OiSeverity]
WHERE IsActive = 1
ORDER BY Id, Name;";
            await using var cn = _factory.Create("AOM");
            var rows = await cn.QueryAsync<OiSeverity>(new CommandDefinition(sql, cancellationToken: ct));
            return rows.AsList();
        }

        public async Task<IReadOnlyList<Site>> GetSitesAsync(CancellationToken ct = default)
        {
            const string sql = "SELECT Id, SiteCode FROM [Employee].[Site] ORDER BY SiteCode;";
            await using var cn = _factory.Create("AOM");
            var rows = await cn.QueryAsync<Site>(new CommandDefinition(sql, cancellationToken: ct));
            return rows.AsList();
        }

        public async Task<OiSeverity?> GetSeverityAsync(int id, CancellationToken ct = default)
        {
            const string sql = @"
SELECT Id, Name, IsActive
FROM [Tools].[OiSeverity]
WHERE Id = @id
ORDER BY Id;";
            await using var cn = _factory.Create("AOM");
            return await cn.QuerySingleOrDefaultAsync<OiSeverity>(
                new CommandDefinition(sql, new { id }, cancellationToken: ct));
        }
    }

    public sealed class OiEventRepository : IOiEventRepository
    {
        private readonly IDbConnectionFactory _factory;
        public OiEventRepository(IDbConnectionFactory f) => _factory = f;

        public async Task<int> InsertAsync(OiEvent e, CancellationToken ct = default)
        {
            const string sql = @"
INSERT INTO [Tools].[OiEvent]
(CategoryId, SeverityId, SiteId, TicketNumber, Summary, ServicesAffected, UsersAffected,
 EstimatedTimeToResolve, PostedTime, StartTime, Description)
OUTPUT INSERTED.Id
VALUES(@CategoryId, @SeverityId, @SiteId, @TicketNumber, @Summary, @ServicesAffected, @UsersAffected,
       @EstimatedTimeToResolve, @PostedTime, @StartTime, @Description);";
            await using var cn = _factory.Create("AOM");
            return await cn.ExecuteScalarAsync<int>(new CommandDefinition(sql, e, cancellationToken: ct));
        }

        public async Task UpdateAsync(OiEvent e, CancellationToken ct = default)
        {
            const string sql = @"
UPDATE [Tools].[OiEvent] SET
    CategoryId = @CategoryId,
    SeverityId = @SeverityId,
    SiteId = @SiteId,
    TicketNumber = @TicketNumber,
    Summary = @Summary,
    ServicesAffected = @ServicesAffected,
    UsersAffected = @UsersAffected,
    EstimatedTimeToResolve = @EstimatedTimeToResolve,
    PostedTime = @PostedTime,
    StartTime = @StartTime,
    ResolutionTime = @ResolutionTime,
    Description = @Description
WHERE Id = @Id;";
            await using var cn = _factory.Create("AOM");
            await cn.ExecuteAsync(new CommandDefinition(sql, new
            {
                e.Id,
                e.CategoryId,
                e.SeverityId,
                e.SiteId,
                e.TicketNumber,
                e.Summary,
                e.ServicesAffected,
                e.UsersAffected,
                e.EstimatedTimeToResolve,
                PostedTime = e.PostedTime,
                StartTime = e.StartTime,
                ResolutionTime = e.ResolutionTime,
                e.Description
            }, cancellationToken: ct));
        }

        public async Task<OiEvent?> GetAsync(int id, CancellationToken ct = default)
        {
            const string sql = "SELECT * FROM [Tools].[OiEvent] WHERE Id = @id;";
            await using var cn = _factory.Create("AOM");
            return await cn.QuerySingleOrDefaultAsync<OiEvent>(
                new CommandDefinition(sql, new { id }, cancellationToken: ct));
        }

        public async Task<IReadOnlyList<OiEvent>> ListAsync(DateTime? fromLocal, DateTime? toLocal, CancellationToken ct = default)
        {
            const string sql = @"
SELECT * FROM [Tools].[OiEvent]
WHERE (@from IS NULL OR PostedTime >= @from)
  AND (@to   IS NULL OR PostedTime <  @to)
ORDER BY PostedTime DESC;";
            await using var cn = _factory.Create("AOM");
            var rows = await cn.QueryAsync<OiEvent>(
                new CommandDefinition(sql, new { from = fromLocal, to = toLocal }, cancellationToken: ct));
            return rows.AsList();
        }

        public async Task<int> InsertUpdateAsync(OiEventUpdate u, bool allClear = false, CancellationToken ct = default)
        {
            const string insertSql = @"
INSERT INTO [Tools].[OiEventUpdate] (EventId, Summary, UpdateTime)
OUTPUT INSERTED.Id
VALUES (@EventId, @Summary, @UpdateTime);";

            const string stampSql = @"
UPDATE [Tools].[OiEvent]
SET    ResolutionTime = @etNow
WHERE  Id = @EventId
  AND  ResolutionTime IS NULL;";

            var etNow = GetEasternNow();

            await using var cn = _factory.Create("AOM");
            await cn.OpenAsync(ct);
            using var tx = cn.BeginTransaction();

            try
            {
                var id = await cn.ExecuteScalarAsync<int>(
                    new CommandDefinition(insertSql, new { u.EventId, u.Summary, UpdateTime = etNow }, tx, cancellationToken: ct));

                if (allClear)
                {
                    await cn.ExecuteAsync(
                        new CommandDefinition(stampSql, new { u.EventId, etNow }, tx, cancellationToken: ct));
                }

                tx.Commit();
                return id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<IReadOnlyList<OiEventUpdate>> ListUpdatesAsync(int eventId, CancellationToken ct = default)
        {
            const string sql = @"
SELECT Id, EventId, Summary, UpdateTime
FROM [Tools].[OiEventUpdate]
WHERE EventId = @eventId
ORDER BY UpdateTime DESC;";
            await using var cn = _factory.Create("AOM");
            var rows = await cn.QueryAsync<OiEventUpdate>(
                new CommandDefinition(sql, new { eventId }, cancellationToken: ct));
            return rows.AsList();
        }

        private static DateTime GetEasternNow()
        {
            TimeZoneInfo tz;
            try { tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); } // Windows
            catch { tz = TimeZoneInfo.FindSystemTimeZoneById("America/New_York"); }   // Linux
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        }
    }
}
