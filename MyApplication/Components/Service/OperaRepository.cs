using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using System.Text.RegularExpressions;
using EmployeeEntity = MyApplication.Components.Model.AOM.Employee.Employees;

namespace MyApplication.Components.Service
{
    public sealed class OperaRepository : IOperaRepository
    {
        private readonly AomDbContext _db;
        public OperaRepository(AomDbContext db) => _db = db;

        public async Task<(IReadOnlyList<OperaRequest> Items, int Total)> SearchAsync(OperaQuery q, CancellationToken ct = default)
        {
            var qry = _db.OperaRequest
                .AsNoTracking()
                .Include(x => x.Employees)
                .Include(x => x.OperaType)
                .Include(x => x.OperaSubType)
                .Include(x => x.OperaSubClass)
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
                // Store status as FK? If so add OperaStatusId to OperaRequest.
                // If you stored as bool Approved & Rejected, map accordingly.
                // Here we assume a FK column: OperaStatusId
                qry = qry.Where(x => EF.Property<int>(x, "OperaStatusId") == q.StatusId.Value);
            }

            if (q.FromUtc.HasValue) qry = qry.Where(x => x.StartTime >= q.FromUtc.Value);
            if (q.ToUtc.HasValue) qry = qry.Where(x => x.StartTime < q.ToUtc.Value);

            var total = await qry.CountAsync(ct);
            var items = await qry.Take(q.Take).ToListAsync(ct);
            return (items, total);
        }

        public Task<OperaRequest?> GetAsync(int requestId, CancellationToken ct = default)
            => _db.OperaRequest
                  .Include(x => x.Employees)
                  .Include(x => x.OperaType)
                  .Include(x => x.OperaSubType)
                  .Include(x => x.OperaSubClass)
                  .FirstOrDefaultAsync(x => x.RequestId == requestId, ct);

        public async Task<int> CreateManyAsync(IEnumerable<OperaRequest> requests, CancellationToken ct = default)
        {
            await _db.OperaRequest.AddRangeAsync(requests, ct);
            return await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(OperaRequest req, CancellationToken ct = default)
        {
            _db.OperaRequest.Update(req);
            await _db.SaveChangesAsync(ct);
        }

        public async Task SetStatusAsync(int requestId, int statusId, string actor, CancellationToken ct = default)
        {
            var r = await _db.OperaRequest.FirstAsync(x => x.RequestId == requestId, ct);
            _db.Entry(r).Property("OperaStatusId").CurrentValue = statusId;
            // Optionally log actor/time in an audit table.
            await _db.SaveChangesAsync(ct);
        }

        public Task<IReadOnlyList<OperaStatus>> GetStatusesAsync(CancellationToken ct = default)
            => _db.OperaStatus.AsNoTracking().OrderBy(x => x.Id).ToListAsync(ct)
               .ContinueWith(t => (IReadOnlyList<OperaStatus>)t.Result, ct);

        public async Task<IReadOnlyList<EmployeeEntity>> SearchEmployeesAsync(string term, int take = 20, CancellationToken ct = default)
        {
            term = (term ?? "").Trim();
            var q = _db.Set<EmployeeEntity>().AsNoTracking();

            if (!string.IsNullOrEmpty(term))
            {
                if (int.TryParse(term, out var id))
                    q = q.Where(e => e.Id == id);
                else
                    q = q.Where(e =>
                        (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{term}%")) ||
                        (e.LastName != null && EF.Functions.Like(e.LastName, $"%{term}%")));
            }

            return await q.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).Take(take).ToListAsync(ct);
        }
    }
}
