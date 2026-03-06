using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Acr;

public sealed partial class AcrQueryService
{
    public async Task<PagedResult<AcrRequestListItem>> SearchAsync(
          AcrIndexFilter filter,
          int pageIndex,
          int pageSize,
          string? sortLabel,
          bool sortDescending,
          CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        // 1. Build Base Query
        var q =
            from r in db.Set<AcrRequest>().AsNoTracking()
            join e in db.Set<Employees>().AsNoTracking() on r.EmployeeId equals e.Id
            join d in db.Set<EmployeeCurrentDetails>().AsNoTracking() on r.EmployeeId equals d.EmployeeId into dd
            from d in dd.DefaultIfEmpty()
            join t in db.Set<AcrType>().AsNoTracking() on r.AcrTypeId equals t.Id into tt
            from t in tt.DefaultIfEmpty()
            join s in db.Set<AcrStatus>().AsNoTracking() on r.AcrStatusId equals s.Id into ss
            from s in ss.DefaultIfEmpty()
            where !filter.ActiveOnly || e.IsActive == true
            select new
            {
                r.Id,
                r.EmployeeId,
                EmployeeName = (e.LastName ?? "") + ", " + (e.FirstName ?? "") + (e.MiddleInitial == null ? "" : " " + e.MiddleInitial),
                e.FirstName,
                e.LastName,
                e.MiddleInitial,
                d.ManagerId,
                d.SupervisorId,
                TypeId = r.AcrTypeId,
                TypeName = t != null ? t.Name : null,
                StatusId = r.AcrStatusId,
                StatusName = s != null ? s.Name : null,
                r.EffectiveDate,
                r.SubmitTime
            };

        // 2. Apply Filters
        if (filter.ManagerId.HasValue)
            q = q.Where(x => x.ManagerId == filter.ManagerId);

        if (filter.SupervisorId.HasValue)
            q = q.Where(x => x.SupervisorId == filter.SupervisorId);

        if (filter.AcrTypeId is int typeId)
            q = q.Where(x => x.TypeId == typeId);

        if (filter.AcrStatusId is int statusId)
            q = q.Where(x => x.StatusId == statusId);

        if (filter.EffectiveFrom is DateOnly from)
            q = q.Where(x => x.EffectiveDate >= from);

        if (filter.EffectiveTo is DateOnly to)
            q = q.Where(x => x.EffectiveDate <= to);

        if (!string.IsNullOrWhiteSpace(filter.EmployeeSearch))
        {
            var search = filter.EmployeeSearch.Trim();
            if (int.TryParse(search, out var empId))
            {
                q = q.Where(x => x.EmployeeId == empId);
            }
            else
            {
                q = q.Where(x =>
                    (x.LastName + ", " + x.FirstName + (x.MiddleInitial == null ? "" : " " + x.MiddleInitial)).Contains(search)
                    || (x.FirstName + " " + x.LastName).Contains(search)
                    || x.FirstName.Contains(search)
                    || x.LastName.Contains(search)
                );
            }
        }

        // 3. Get Total Count
        var totalCount = await q.CountAsync(ct);

        // 4. Apply Sorting
        if (string.IsNullOrEmpty(sortLabel))
        {
            q = q.OrderByDescending(x => x.SubmitTime);
        }
        else
        {
            q = sortLabel switch
            {
                "EmployeeName"  => sortDescending ? q.OrderByDescending(x => x.EmployeeName)  : q.OrderBy(x => x.EmployeeName),
                "AcrType"       => sortDescending ? q.OrderByDescending(x => x.TypeName)       : q.OrderBy(x => x.TypeName),
                "AcrStatus"     => sortDescending ? q.OrderByDescending(x => x.StatusName)     : q.OrderBy(x => x.StatusName),
                "EffectiveDate" => sortDescending ? q.OrderByDescending(x => x.EffectiveDate)  : q.OrderBy(x => x.EffectiveDate),
                "SubmitTime"    => sortDescending ? q.OrderByDescending(x => x.SubmitTime)     : q.OrderBy(x => x.SubmitTime),
                _               => q.OrderByDescending(x => x.SubmitTime)
            };
        }

        // 5. Apply Paging
        var data = await q
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // 6. Map
        var items = data.Select(x => new AcrRequestListItem(
            x.Id,
            x.EmployeeId,
            x.EmployeeName,
            x.TypeName ?? "",
            x.StatusName,
            x.EffectiveDate,
            x.SubmitTime
        )).ToList();

        return new PagedResult<AcrRequestListItem>(items, totalCount);
    }

    // ===== Index query (No Paging - for Exports) =====
    public async Task<List<AcrRequestListItem>> QueryAsync(
        AcrIndexFilter filter,
        int take = 1000,
        CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var q =
            from r in db.Set<AcrRequest>().AsNoTracking()
            join e in db.Set<Employees>().AsNoTracking() on r.EmployeeId equals e.Id
            join t in db.Set<AcrType>().AsNoTracking() on r.AcrTypeId equals t.Id into tt
            from t in tt.DefaultIfEmpty()
            join s in db.Set<AcrStatus>().AsNoTracking() on r.AcrStatusId equals s.Id into ss
            from s in ss.DefaultIfEmpty()
            where !filter.ActiveOnly || e.IsActive == true
            select new
            {
                r.Id,
                r.EmployeeId,
                EmployeeName = (e.LastName ?? "") + ", " + (e.FirstName ?? "") + (e.MiddleInitial == null ? "" : " " + e.MiddleInitial),
                TypeId = r.AcrTypeId,
                TypeName = t != null ? t.Name : null,
                StatusId = r.AcrStatusId,
                StatusName = s != null ? s.Name : null,
                r.EffectiveDate,
                r.SubmitTime
            };

        if (filter.AcrTypeId is int typeId)
            q = q.Where(x => x.TypeId == typeId);

        if (filter.AcrStatusId is int statusId)
            q = q.Where(x => x.StatusId == statusId);

        if (filter.EffectiveFrom is DateOnly from)
            q = q.Where(x => x.EffectiveDate >= from);

        if (filter.EffectiveTo is DateOnly to)
            q = q.Where(x => x.EffectiveDate <= to);

        if (!string.IsNullOrWhiteSpace(filter.EmployeeSearch))
        {
            var search = filter.EmployeeSearch.Trim();
            if (int.TryParse(search, out var empId))
            {
                q = q.Where(x => x.EmployeeId == empId);
            }
            else
            {
                var tokens = search.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (tokens.Length == 1)
                {
                    var p = tokens[0];
                    q = q.Where(x => x.EmployeeName.Contains(p));
                }
                else
                {
                    var p1 = tokens[0];
                    var p2 = tokens[1];
                    q = q.Where(x => x.EmployeeName.Contains(p1) && x.EmployeeName.Contains(p2));
                }
            }
        }

        var rows = await q
            .OrderByDescending(x => x.SubmitTime)
            .ThenByDescending(x => x.EffectiveDate)
            .Take(take)
            .ToListAsync(ct);

        return rows.Select(x =>
            new AcrRequestListItem(
                x.Id,
                x.EmployeeId,
                x.EmployeeName,
                x.TypeName ?? "",
                x.StatusName,
                x.EffectiveDate,
                x.SubmitTime)
        ).ToList();
    }
}
