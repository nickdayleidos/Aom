using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Acr;

public sealed record AcrRequestListItem(
    int Id,
    int EmployeeId,
    string EmployeeName,
    string AcrType,
    string? AcrStatus,
    DateOnly EffectiveDate,
    DateTime? SubmitTime
);

// Make filters nullable so "All" is natural (null = no filter)
public sealed record AcrIndexFilter(
    int? AcrTypeId,
    int? AcrStatusId,
    string? EmployeeSearch,   // id or "first last"
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo
);

public interface IAcrQueryService
{
    Task<List<AcrRequestListItem>> QueryAsync(AcrIndexFilter filter, CancellationToken ct = default);

    Task<Dictionary<int, string>> GetTypesAsync(CancellationToken ct = default);
    Task<Dictionary<int, string>> GetStatusesAsync(CancellationToken ct = default);

    Task<List<Employees>> GetEmployeesLookupAsync(CancellationToken ct = default);
    Task<List<Employees>> GetManagerEmployeesAsync(CancellationToken ct = default);
    Task<List<Employees>> GetSupervisorEmployeesAsync(CancellationToken ct = default);

    Task<List<Organization>> GetOrganizationsAsync(CancellationToken ct = default);
    Task<List<SubOrganization>> GetSubOrganizationsAsync(CancellationToken ct = default);
    Task<List<Site>> GetSitesAsync(CancellationToken ct = default);
    Task<List<Employer>> GetEmployersAsync(CancellationToken ct = default);
}

public sealed class AcrQueryService : IAcrQueryService
{
    private readonly AomDbContext _db;
    public AcrQueryService(AomDbContext db) => _db = db;

    // ----- Lookups -----
    public async Task<Dictionary<int, string>> GetTypesAsync(CancellationToken ct = default)
        => await _db.Set<AcrType>().AsNoTracking()
            .OrderBy(x => x.Name)
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

    public async Task<Dictionary<int, string>> GetStatusesAsync(CancellationToken ct = default)
        => await _db.Set<AcrStatus>().AsNoTracking()
            .OrderBy(x => x.Name)
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

    public async Task<List<Employees>> GetEmployeesLookupAsync(CancellationToken ct = default)
        => await _db.Set<Employees>().AsNoTracking()
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new Employees { Id = e.Id, FirstName = e.FirstName, LastName = e.LastName })
            .ToListAsync(ct);

    public async Task<List<Employees>> GetManagerEmployeesAsync(CancellationToken ct = default)
        => await (from m in _db.Set<Manager>().AsNoTracking()
                  join e in _db.Set<Employees>().AsNoTracking() on m.EmployeeId equals e.Id
                  where m.IsActive
                  orderby e.LastName, e.FirstName
                  select new Employees { Id = e.Id, FirstName = e.FirstName, LastName = e.LastName })
                 .ToListAsync(ct);

    public async Task<List<Employees>> GetSupervisorEmployeesAsync(CancellationToken ct = default)
        => await (from s in _db.Set<Supervisor>().AsNoTracking()
                  join e in _db.Set<Employees>().AsNoTracking() on s.EmployeeId equals e.Id
                  where s.IsActive
                  orderby e.LastName, e.FirstName
                  select new Employees { Id = e.Id, FirstName = e.FirstName, LastName = e.LastName })
                 .ToListAsync(ct);

    public async Task<List<Organization>> GetOrganizationsAsync(CancellationToken ct = default)
        => await _db.Set<Organization>().AsNoTracking()
            .OrderBy(o => o.Name)
            .ToListAsync(ct);

    public async Task<List<SubOrganization>> GetSubOrganizationsAsync(CancellationToken ct = default)
        => await _db.Set<SubOrganization>().AsNoTracking()
            .OrderBy(o => o.Name)
            .ToListAsync(ct);

    public async Task<List<Site>> GetSitesAsync(CancellationToken ct = default)
        => await _db.Set<Site>().AsNoTracking()
            .OrderBy(s => s.SiteCode).ThenBy(s => s.Location)
            .ToListAsync(ct);

    public async Task<List<Employer>> GetEmployersAsync(CancellationToken ct = default)
        => await _db.Set<Employer>().AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(ct);

    // ----- Index query -----
    public async Task<List<AcrRequestListItem>> QueryAsync(AcrIndexFilter f, CancellationToken ct = default)
    {
        var q = _db.Set<AcrRequest>()
                   .AsNoTracking()
                   .Include(r => r.AcrStatus)
                   .Include(r => r.Employee)
                   .Include(r => r.AcrType)
                   .AsQueryable();

        if (f.AcrTypeId is int t) q = q.Where(r => r.AcrTypeId == t);
        if (f.AcrStatusId is int s) q = q.Where(r => r.AcrStatusId == s);

        if (!string.IsNullOrWhiteSpace(f.EmployeeSearch))
        {
            var sText = f.EmployeeSearch.Trim();
            if (int.TryParse(sText, out var empId))
            {
                q = q.Where(r => r.EmployeeId == empId);
            }
            else
            {
                var pat = $"%{sText.Replace('%', ' ').Replace('_', ' ').Trim()}%";
                q = q.Where(r =>
                    EF.Functions.Like(r.Employee!.FirstName ?? "", pat) ||
                    EF.Functions.Like(r.Employee!.LastName ?? "", pat));
            }
        }

        if (f.EffectiveFrom is DateOnly df) q = q.Where(r => r.EffectiveDate >= df);
        if (f.EffectiveTo is DateOnly dt) q = q.Where(r => r.EffectiveDate <= dt);

        return await q
            .OrderByDescending(r => r.SubmitTime)
            .Select(r => new AcrRequestListItem(
                r.Id,
                r.EmployeeId,
                (((r.Employee!.FirstName ?? string.Empty) + " " + (r.Employee!.LastName ?? string.Empty)).Trim()),
                r.AcrType!.Name,
                r.AcrStatus!.Name,
                r.EffectiveDate,
                r.SubmitTime))
            .ToListAsync(ct);
    }
}
