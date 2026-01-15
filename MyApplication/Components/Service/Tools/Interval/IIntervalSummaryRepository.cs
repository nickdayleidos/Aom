using Dapper;
using Microsoft.Data.SqlClient;
using MyApplication.Components.Model.AOM;
using MyApplication.Components.Model.AOM.Tools;

public interface IIntervalSummaryRepository
{
    Task<int> InsertAsync(IntervalSummary row, CancellationToken ct = default);
    Task<IntervalSummary?> GetLatestAsync(CancellationToken ct = default);
}