using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Common.Time; // For Et.Now
using System.Text.RegularExpressions;
using EmployeeEntity = MyApplication.Components.Model.AOM.Employee.Employees;

namespace MyApplication.Components.Service
{
    public sealed class OperaRepository : IOperaRepository
    {
        private readonly IDbContextFactory<AomDbContext> _factory;

        private const int SubmittedStatusId = 1;
        private const int ApprovedStatusId = 2;
        private const int CancelledStatusId = 4;
        private const int RejectedStatusId = 5;

        // INJECT FACTORY, NOT DBCONTEXT
        public OperaRepository(IDbContextFactory<AomDbContext> factory) => _factory = factory;

        public async Task<(IReadOnlyList<OperaRequest> Items, int Total)> SearchAsync(OperaQuery q, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var qry = db.OperaRequests
                .AsNoTracking()
                .Include(x => x.Employees)
                .Include(x => x.ActivityType)
                .Include(x => x.ActivitySubType)
                .OrderByDescending(x => x.StartTime)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.NameOrId))
            {
                var t = q.NameOrId.Trim();
                if (int.TryParse(t, out var asInt))
                {
                    qry = qry.Where(x => x.EmployeeId == asInt || x.RequestId == asInt);
                }
                else
                {
                    t = Regex.Replace(t, @"\s+", " ");
                    qry = qry.Where(x =>
                        (x.Employees.FirstName != null && EF.Functions.Like(x.Employees.FirstName, $"%{t}%")) ||
                        (x.Employees.LastName != null && EF.Functions.Like(x.Employees.LastName, $"%{t}%")));
                }
            }

            if (q.StatusId.HasValue)
            {
                qry = qry.Where(x => x.OperaStatusId == q.StatusId.Value);
            }

            if (q.FromUtc.HasValue) qry = qry.Where(x => x.StartTime >= q.FromUtc.Value);
            if (q.ToUtc.HasValue) qry = qry.Where(x => x.StartTime < q.ToUtc.Value);

            var total = await qry.CountAsync(ct);
            var items = await qry.Take(q.Take).ToListAsync(ct);

            return (items, total);
        }

        public async Task<OperaRequest?> GetAsync(int requestId, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.OperaRequests
                  .Include(x => x.Employees)
                  .Include(x => x.ActivityType)
                  .Include(x => x.ActivitySubType)
                  .FirstOrDefaultAsync(x => x.RequestId == requestId, ct);
        }

        public async Task<int> CreateManyAsync(IEnumerable<OperaRequest> requests, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            await db.OperaRequests.AddRangeAsync(requests, ct);
            return await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(OperaRequest req, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            db.OperaRequests.Update(req);
            await db.SaveChangesAsync(ct);
        }

        public async Task SetStatusAsync(int requestId, int statusId, string actor, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var r = await db.OperaRequests.FirstOrDefaultAsync(x => x.RequestId == requestId, ct);
            if (r == null) return;

            r.OperaStatusId = statusId;
            var nowEt = Et.Now;

            if (statusId == ApprovedStatusId)
            {
                r.ApproveTime = nowEt;
                r.ApproveBy = actor;
            }
            else if (statusId == RejectedStatusId)
            {
                r.RejectedTime = nowEt;
                r.RejectedBy = actor;
            }
            else if (statusId == CancelledStatusId)
            {
                r.CancelledTime = nowEt;
                r.CancelledBy = actor;
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<OperaStatus>> GetStatusesAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.OperaStatuses.AsNoTracking().OrderBy(x => x.Id).ToListAsync(ct);
        }

        public async Task<IReadOnlyList<EmployeeEntity>> SearchEmployeesAsync(string term, int take = 20, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            IQueryable<EmployeeEntity> q = db.Employees.AsNoTracking().Where(e => e.IsActive == true);

            if (!string.IsNullOrWhiteSpace(term))
            {
                if (int.TryParse(term, out var id))
                {
                    q = q.Where(e => e.Id == id);
                }
                else
                {
                    q = q.Where(e =>
                        (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{term}%")) ||
                        (e.LastName != null && EF.Functions.Like(e.LastName, $"%{term}%")));
                }
            }

            return await q.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).Take(take).ToListAsync(ct);
        }

        // --- NEW HELPER IMPLEMENTATIONS ---

        public async Task<Dictionary<int, string>> GetActivityTypesAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.ActivityTypes.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        }

        public async Task<Dictionary<int, string>> GetActivitySubTypesAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.ActivitySubTypes.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        }

        public async Task<Dictionary<int, string?>> GetEmployeeSiteTimeZonesAsync(IEnumerable<int> employeeIds, CancellationToken ct = default)
        {
            var distinctIds = employeeIds.Distinct().ToList();
            if (distinctIds.Count == 0) return new Dictionary<int, string?>();

            await using var db = await _factory.CreateDbContextAsync(ct);

            var histories = await db.EmployeeHistory
                .AsNoTracking()
                .Include(h => h.Site)
                .Where(h => distinctIds.Contains(h.EmployeeId))
                .Select(h => new { h.EmployeeId, h.EffectiveDate, h.IsActive, h.Site })
                .ToListAsync(ct);

            return histories
                .GroupBy(h => h.EmployeeId)
                .ToDictionary(g => g.Key, g =>
                {
                    // Prefer active/most recent
                    var candidate = g.Where(h => h.IsActive == true)
                                     .OrderByDescending(h => h.EffectiveDate).FirstOrDefault()
                                     ?? g.OrderByDescending(h => h.EffectiveDate).FirstOrDefault();

                    // Return Windows ID or IANA ID
                    return candidate?.Site?.TimeZoneWindows ?? candidate?.Site?.TimeZoneIana;
                });
        }
    }
}