using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Acr;

public sealed partial class AcrQueryService
{
    public async Task<List<KeyValuePair<int, string>>> GetOrganizationsAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Organizations.AsNoTracking()
            .Where(o => o.IsActive == true)
            .OrderBy(o => o.Name)
            .Select(o => new KeyValuePair<int, string>(o.Id, o.Name))
            .ToListAsync(ct);
    }

    public async Task<List<KeyValuePair<int, string>>> GetSubOrganizationsAsync(int? orgId, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var q = db.SubOrganizations.AsNoTracking();

        if (orgId is int id)
            q = q.Where(s => s.OrganizationId == id || s.OrganizationId == null);
        else
            q = q.Where(s => s.OrganizationId == null);

        return await q.OrderBy(s => s.Name)
                      .Where(s => s.IsActive == true)
                      .Select(s => new KeyValuePair<int, string>(s.Id, s.Name))
                      .ToListAsync(ct);
    }

    public async Task<List<KeyValuePair<int, string>>> GetSitesAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Sites.AsNoTracking()
            .Where(s => s.IsActive == true)
            .OrderBy(s => s.SiteCode)
            .Select(s => new KeyValuePair<int, string>(s.Id, s.SiteCode))
            .ToListAsync(ct);
    }

    public async Task<List<KeyValuePair<int, string>>> GetEmployersAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Employers.AsNoTracking()
            .Where(e => e.IsActive == true)
            .OrderBy(e => e.Name)
            .Select(e => new KeyValuePair<int, string>(e.Id, e.Name))
            .ToListAsync(ct);
    }

    public async Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var data = await (
            from m in db.Managers.AsNoTracking()
            join e in db.Employees.AsNoTracking() on m.EmployeeId equals e.Id
            where m.IsActive == true && e.IsActive == true
            orderby e.LastName, e.FirstName
            select new { m.Id, e.LastName, e.FirstName, e.MiddleInitial }
        ).ToListAsync(ct);

        return data
            .Select(x => new KeyValuePair<int, string>(
                x.Id,
                $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"))
            .ToList();
    }

    public async Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var data = await (
            from s in db.Supervisors.AsNoTracking()
            join e in db.Employees.AsNoTracking() on s.EmployeeId equals e.Id
            where s.IsActive == true && e.IsActive == true
            orderby e.LastName, e.FirstName
            select new { s.Id, e.LastName, e.FirstName, e.MiddleInitial }
        ).ToListAsync(ct);

        return data
            .Select(x => new KeyValuePair<int, string>(
                x.Id,
                $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"))
            .ToList();
    }

    public async Task<List<KeyValuePair<int, string>>> SearchManagersAsync(
        string? text, int take = 25, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var q = from m in db.Managers.AsNoTracking()
                join e in db.Employees.AsNoTracking() on m.EmployeeId equals e.Id
                where m.IsActive == true && e.IsActive == true
                select new { m.Id, e.LastName, e.FirstName, e.MiddleInitial };

        if (!string.IsNullOrWhiteSpace(text))
            q = q.Where(x => x.LastName.Contains(text) || x.FirstName.Contains(text));

        var data = await q.OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                          .Take(take)
                          .ToListAsync(ct);

        return data.Select(x => new KeyValuePair<int, string>(
            x.Id,
            $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"))
            .ToList();
    }

    public async Task<List<KeyValuePair<int, string>>> SearchSupervisorsAsync(
        string? text, int take = 25, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var q = from s in db.Supervisors.AsNoTracking()
                join e in db.Employees.AsNoTracking() on s.EmployeeId equals e.Id
                where s.IsActive == true && e.IsActive == true
                select new { s.Id, e.LastName, e.FirstName, e.MiddleInitial };

        if (!string.IsNullOrWhiteSpace(text))
            q = q.Where(x => x.LastName.Contains(text) || x.FirstName.Contains(text));

        var data = await q.OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                          .Take(take)
                          .ToListAsync(ct);

        return data.Select(x => new KeyValuePair<int, string>(
            x.Id,
            $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"))
            .ToList();
    }

    public async Task<Dictionary<int, string>> GetTypesAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Set<AcrType>().AsNoTracking()
            .Where(x => x.IsActive == true)
            .OrderBy(x => x.Name)
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
    }

    public async Task<Dictionary<int, string>> GetStatusesAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Set<AcrStatus>().AsNoTracking()
            .OrderBy(x => x.Name)
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
    }

    public async Task<List<Employees>> GetEmployeesLookupAsync(bool? isActive, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var q = db.Employees.AsQueryable();

        if (isActive is true)  q = q.Where(e => e.IsActive);
        if (isActive is false) q = q.Where(e => !e.IsActive);

        return await q
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .ToListAsync(ct);
    }

    public async Task<List<KeyValuePair<int, string>>> GetOvertimeTypesAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Set<AcrOvertimeTypes>().AsNoTracking()
            .Where(x => x.IsActive == true)
            .OrderBy(x => x.Name)
            .Select(x => new KeyValuePair<int, string>(x.Id, x.Name))
            .ToListAsync(ct);
    }

    public Task<EmployeeHistorySnapshot?> GetLatestEmployeeHistoryAsync(
        int employeeId,
        CancellationToken ct = default)
        => Task.FromResult<EmployeeHistorySnapshot?>(null);

    public async Task<string?> GetStatusNameByAcrIdAsync(int acrRequestId, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await (
            from r in db.Set<AcrRequest>().AsNoTracking()
            join s in db.Set<AcrStatus>().AsNoTracking() on r.AcrStatusId equals s.Id
            where r.Id == acrRequestId
            select s.Name
        ).FirstOrDefaultAsync(ct);
    }

    public async Task<string?> GetSiteTimeZoneAsync(int siteId, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Sites.AsNoTracking()
            .Where(s => s.Id == siteId)
            .Select(s => s.TimeZoneWindows)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Employees?> GetEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId, ct);
    }

    public async Task<int?> GetStatusIdByAcrIdAsync(int acrId, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.AcrRequests
            .Where(r => r.Id == acrId)
            .Select(r => (int?)r.AcrStatusId)
            .FirstOrDefaultAsync(ct);
    }
}
