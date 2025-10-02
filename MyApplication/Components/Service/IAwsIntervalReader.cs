// Services/Intervals/IAwsIntervalReader.cs
using Dapper;
using MyApplication.Components.Services.Email;

public interface IAwsIntervalReader
{
    Task<List<IntervalRow>> GetIntervalsAsync(DateOnly date, CancellationToken ct = default);
    // add any other “populate” queries you do today
}

// Services/Intervals/AwsIntervalReader.cs


public sealed class AwsIntervalReader : IAwsIntervalReader
{
    private readonly IDbConnectionFactory _factory;
    public AwsIntervalReader(IDbConnectionFactory factory) => _factory = factory;

    public async Task<List<IntervalRow>> GetIntervalsAsync(DateOnly date, CancellationToken ct = default)
    {
        await using var conn = _factory.Create("AWS");
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var sql = @"
SELECT
  FORMAT([interval], 'hh\:mm') AS IntervalLabel,
  callsoffered   AS CallsOffered,
  acdcalls       AS Answered,
  CAST(anstime / NULLIF(acdcalls,0) AS int) AS ASA  -- example ASA calc
FROM dbo.AwsIntervalData
WHERE et >= @start AND et < @end
ORDER BY [interval];";

        var rows = await conn.QueryAsync<IntervalRow>(sql, new { start, end });
        return rows.ToList();
    }
}
