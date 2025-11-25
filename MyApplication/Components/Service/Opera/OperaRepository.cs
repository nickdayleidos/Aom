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
        private const int SubmittedStatusId = 1;
        private const int ApprovedStatusId = 2;
        private const int ProcessedStatusId = 3;
        private const int CancelledStatusId = 4;
        private const int RejectedStatusId = 5;


        public OperaRepository(AomDbContext db) => _db = db;

        public async Task<(IReadOnlyList<OperaRequest> Items, int Total)> SearchAsync(OperaQuery q, CancellationToken ct = default)
        {
            var qry = _db.OperaRequests
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
            => _db.OperaRequests
                  .Include(x => x.Employees)
                  .Include(x => x.ActivityType)
                  .Include(x => x.ActivitySubType)
                  
                  .FirstOrDefaultAsync(x => x.RequestId == requestId, ct);

        public async Task<int> CreateManyAsync(IEnumerable<OperaRequest> requests, CancellationToken ct = default)
        {
            await _db.OperaRequests.AddRangeAsync(requests, ct);
            return await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(OperaRequest req, CancellationToken ct = default)
        {
            _db.OperaRequests.Update(req);
            await _db.SaveChangesAsync(ct);
        }

        public async Task SetStatusAsync(int requestId, int statusId, string actor, CancellationToken ct = default)
        {
            var r = await _db.OperaRequests.FirstAsync(x => x.RequestId == requestId, ct);

            r.OperaStatusId = statusId;

            var nowEt = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

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

            await _db.SaveChangesAsync(ct);
        }


        public Task<IReadOnlyList<OperaStatus>> GetStatusesAsync(CancellationToken ct = default)
            => _db.OperaStatuses.AsNoTracking().OrderBy(x => x.Id).ToListAsync(ct)
               .ContinueWith(t => (IReadOnlyList<OperaStatus>)t.Result, ct);

        public async Task<IReadOnlyList<EmployeeEntity>> SearchEmployeesAsync(
    string term,
    int take = 20,
    CancellationToken ct = default)
        {
            // Only active employees
            IQueryable<EmployeeEntity> q = _db.Employees
                .AsNoTracking()
                .Where(e => e.IsActive == true);

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

            return await q
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
