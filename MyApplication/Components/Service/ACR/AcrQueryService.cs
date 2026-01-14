using Microsoft.EntityFrameworkCore;
using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Service.Acr;
using MyApplication.Components.Service.Employee;
using MyApplication.Migrations;
using System;

namespace MyApplication.Components.Service.Acr;

public sealed class AcrQueryService : IAcrQueryService
{

    private readonly IDbContextFactory<AomDbContext> _dbFactory;
    private readonly ITimeDisplayService _timeService;

    public AcrQueryService(
        IDbContextFactory<AomDbContext> dbFactory,
        ITimeDisplayService timeService)
    {
        _dbFactory = dbFactory;
        _timeService = timeService;
    }

    // ========= Lookups used by forms/details =========
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
                // FILTER: Apply Active Only logic
            where !filter.ActiveOnly || e.IsActive == true
            select new
            {
                r.Id,
                r.EmployeeId,

                // Projection for Display (Last, First MI)
                EmployeeName = (e.LastName ?? "") + ", " + (e.FirstName ?? "") + (e.MiddleInitial == null ? "" : " " + e.MiddleInitial),

                // Helper columns for Searching (Required to support "First Last" logic in the Where clause)
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
                // Updated Logic: Check both formats
                q = q.Where(x =>
                    // "Last, First MI" (matches the display format)
                    (x.LastName + ", " + x.FirstName + (x.MiddleInitial == null ? "" : " " + x.MiddleInitial)).Contains(search)
                    ||
                    // "First Last"
                    (x.FirstName + " " + x.LastName).Contains(search)
                    ||
                    // Simple partial matches
                    x.FirstName.Contains(search)
                    ||
                    x.LastName.Contains(search)
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
                "EmployeeName" => sortDescending ? q.OrderByDescending(x => x.EmployeeName) : q.OrderBy(x => x.EmployeeName),
                "AcrType" => sortDescending ? q.OrderByDescending(x => x.TypeName) : q.OrderBy(x => x.TypeName),
                "AcrStatus" => sortDescending ? q.OrderByDescending(x => x.StatusName) : q.OrderBy(x => x.StatusName),
                "EffectiveDate" => sortDescending ? q.OrderByDescending(x => x.EffectiveDate) : q.OrderBy(x => x.EffectiveDate),
                "SubmitTime" => sortDescending ? q.OrderByDescending(x => x.SubmitTime) : q.OrderBy(x => x.SubmitTime),
                _ => q.OrderByDescending(x => x.SubmitTime)
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
                // FILTER: Apply Active Only logic (Was missing here)
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
        {
            q = q.Where(s => s.OrganizationId == id || s.OrganizationId == null);
        }
        else
        {
            q = q.Where(s => s.OrganizationId == null);
        }

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
            where m.IsActive == true && e.IsActive == true  // <--- Ensure Employee is Active
            orderby e.LastName, e.FirstName
            select new
            {
                m.Id,
                e.LastName,
                e.FirstName,
                e.MiddleInitial
            }
        ).ToListAsync(ct);

        return data
            .Select(x => new KeyValuePair<int, string>(
                x.Id,
                $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"
            ))
            .ToList();
    }
    public async Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var data = await (
            from s in db.Supervisors.AsNoTracking()
            join e in db.Employees.AsNoTracking() on s.EmployeeId equals e.Id
            where s.IsActive == true && e.IsActive == true  // <--- Ensure Employee is Active
            orderby e.LastName, e.FirstName
            select new
            {
                s.Id,
                e.LastName,
                e.FirstName,
                e.MiddleInitial
            }
        ).ToListAsync(ct);

        return data
            .Select(x => new KeyValuePair<int, string>(
                x.Id,
                $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"
            ))
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

        if (isActive is true) q = q.Where(e => e.IsActive);
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

    // ===== Details (read-only) =====
    public async Task<AcrDetailsVm> LoadDetailsAsync(int id, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var req = await db.Set<AcrRequest>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new InvalidOperationException($"ACR {id} not found.");

        var empName = await db.Set<Employees>()
            .AsNoTracking()
            .Where(e => e.Id == req.EmployeeId)
            .Select(e => ((e.LastName ?? string.Empty) + ", " + (e.FirstName ?? string.Empty) + (e.MiddleInitial == null ? "" : " " + e.MiddleInitial)))
            .FirstOrDefaultAsync(ct) ?? string.Empty;

        var org = await db.Set<AcrOrganization>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AcrRequestId == id, ct);

        OrganizationChangeDto? orgDto = org is null
            ? null
            : new OrganizationChangeDto(
                org.OrganizationId, org.SubOrganizationId, org.SiteId, org.EmployerId,
                org.ManagerId, org.SupervisorId, org.IsLoa, org.IsIntLoa, org.IsRemote);

        var sch = await db.Set<AcrSchedule>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AcrRequestId == id && x.ShiftNumber == 1, ct);

        ScheduleChangeDto? schDto = sch is null
            ? null
            : new ScheduleChangeDto
            {
                IsSplitSchedule = sch.IsSplitSchedule,
                ShiftNumber = sch.ShiftNumber ?? 1,

                MondayStart = sch.MondayStart,
                MondayEnd = sch.MondayEnd,
                TuesdayStart = sch.TuesdayStart,
                TuesdayEnd = sch.TuesdayEnd,
                WednesdayStart = sch.WednesdayStart,
                WednesdayEnd = sch.WednesdayEnd,
                ThursdayStart = sch.ThursdayStart,
                ThursdayEnd = sch.ThursdayEnd,
                FridayStart = sch.FridayStart,
                FridayEnd = sch.FridayEnd,
                SaturdayStart = sch.SaturdayStart,
                SaturdayEnd = sch.SaturdayEnd,
                SundayStart = sch.SundayStart,
                SundayEnd = sch.SundayEnd,

                IsStaticBreakSchedule = sch.IsStaticBreakSchedule,
                IsOtAdjustment = sch.IsOtAdjustment
            };

        var ot = await db.Set<AcrOvertimeSchedules>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AcrRequestId == id, ct);

        OvertimeAdjustmentDto? otDto = ot is null
            ? null
            : new OvertimeAdjustmentDto(
                ot.MondayTypeId, ot.TuesdayTypeId, ot.WednesdayTypeId,
                ot.ThursdayTypeId, ot.FridayTypeId, ot.SaturdayTypeId, ot.SundayTypeId);

        var typeId = req.AcrTypeId
            ?? throw new InvalidOperationException($"ACR {id} has null AcrTypeId.");

        return new AcrDetailsVm(
            req.Id,
            (AcrTypeKey)typeId,
            req.EmployeeId,
            empName,
            req.EffectiveDate,
            req.SubmitterComment ?? string.Empty,
            orgDto,
            schDto,
            otDto
        );
    }

    public Task<EmployeeHistorySnapshot?> GetLatestEmployeeHistoryAsync(
        int employeeId,
        CancellationToken ct = default)
        => Task.FromResult<EmployeeHistorySnapshot?>(null);

    public async Task<string?> GetStatusNameByAcrIdAsync(int acrRequestId, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var name = await (
            from r in db.Set<AcrRequest>().AsNoTracking()
            join s in db.Set<AcrStatus>().AsNoTracking() on r.AcrStatusId equals s.Id
            where r.Id == acrRequestId
            select s.Name
        ).FirstOrDefaultAsync(ct);

        return name;
    }

    public async Task<AcrEditVm> LoadForEditAsync(int id, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var req = await db.Set<AcrRequest>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new InvalidOperationException($"ACR {id} not found.");

        var org = await db.Set<AcrOrganization>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AcrRequestId == id, ct);

        var sch = await db.Set<AcrSchedule>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AcrRequestId == id && x.ShiftNumber == 1, ct);

        var ot = await db.Set<AcrOvertimeSchedules>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AcrRequestId == id, ct);

        OrganizationChangeDto? orgDto = org is null
            ? null
            : new OrganizationChangeDto(
                org.OrganizationId, org.SubOrganizationId, org.SiteId, org.EmployerId,
                org.ManagerId, org.SupervisorId, org.IsLoa, org.IsIntLoa, org.IsRemote);

        ScheduleChangeDto? schDto = null;

        if (sch != null)
        {
            string? tzId = null;

            if (org?.SiteId != null)
            {
                tzId = await db.Sites.AsNoTracking()
                    .Where(s => s.Id == org.SiteId)
                    .Select(s => s.TimeZoneWindows)
                    .FirstOrDefaultAsync(ct);
            }
            else
            {
                // FIX: Get SiteId from EmployeeHistory instead of Employees table
                var currentSiteId = await db.EmployeeHistory
                    .AsNoTracking()
                    .Where(h => h.EmployeeId == req.EmployeeId)
                    .OrderByDescending(h => h.EffectiveDate)
                    .ThenByDescending(h => h.Id)
                    .Select(h => h.SiteId)
                    .FirstOrDefaultAsync(ct);

                if (currentSiteId != null)
                {
                    tzId = await db.Sites.AsNoTracking()
                        .Where(s => s.Id == currentSiteId)
                        .Select(s => s.TimeZoneWindows)
                        .FirstOrDefaultAsync(ct);
                }
            }

            TimeOnly? ToLoc(TimeOnly? et) => et.HasValue ? _timeService.ToLocal(et.Value, tzId) : null;

            schDto = new ScheduleChangeDto
            {
                IsSplitSchedule = sch.IsSplitSchedule,
                ShiftNumber = sch.ShiftNumber ?? 1,

                MondayStart = ToLoc(sch.MondayStart),
                MondayEnd = ToLoc(sch.MondayEnd),
                TuesdayStart = ToLoc(sch.TuesdayStart),
                TuesdayEnd = ToLoc(sch.TuesdayEnd),
                WednesdayStart = ToLoc(sch.WednesdayStart),
                WednesdayEnd = ToLoc(sch.WednesdayEnd),
                ThursdayStart = ToLoc(sch.ThursdayStart),
                ThursdayEnd = ToLoc(sch.ThursdayEnd),
                FridayStart = ToLoc(sch.FridayStart),
                FridayEnd = ToLoc(sch.FridayEnd),
                SaturdayStart = ToLoc(sch.SaturdayStart),
                SaturdayEnd = ToLoc(sch.SaturdayEnd),
                SundayStart = ToLoc(sch.SundayStart),
                SundayEnd = ToLoc(sch.SundayEnd),

                IsStaticBreakSchedule = sch.IsStaticBreakSchedule,
                IsOtAdjustment = sch.IsOtAdjustment
            };
        }

        OvertimeAdjustmentDto? otDto = ot is null
            ? null
            : new OvertimeAdjustmentDto(
                ot.MondayTypeId, ot.TuesdayTypeId, ot.WednesdayTypeId,
                ot.ThursdayTypeId, ot.FridayTypeId, ot.SaturdayTypeId, ot.SundayTypeId);

        var typeId = req.AcrTypeId
            ?? throw new InvalidOperationException($"ACR {id} has null AcrTypeId.");

        return new AcrEditVm
        {
            Id = req.Id,
            EmployeeId = req.EmployeeId,
            TypeId = typeId,
            EffectiveDate = req.EffectiveDate,
            SubmitterComment = req.SubmitterComment,
            Organization = orgDto,
            Schedule = schDto,
            Overtime = otDto,
            IncludeOvertimeAdjustment = (sch?.IsOtAdjustment == true) || otDto is not null
        };
    }

    // Note: PrevDetailsVm is assumed to be in AcrDtos.cs now, removed from here
    public async Task<PrevDetailsVm?> GetPrevDetailsAsync(
        int employeeId,
        TimeDisplayMode mode,
        CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var eh = await db.EmployeeHistory
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.EffectiveDate)
            .Select(x => new
            {
                x.EmployeeId,
                x.EffectiveDate,
                x.EmployerId,
                x.SiteId,
                x.OrganizationId,
                x.SubOrganizationId,
                x.ManagerId,
                x.SupervisorId,
                x.IsActive,
                x.IsLoa,
                x.IsIntLoa,
                x.IsRemote,
                x.ScheduleRequestId,
                x.OvertimeRequestId
            })
            .FirstOrDefaultAsync(ct);

        if (eh is null) return null;

        string? employerName = null,
                siteCode = null,
                siteWindowsTz = null,
                orgName = null,
                subOrgName = null,
                mgrName = null,
                supName = null;

        if (eh.EmployerId is int empId)
        {
            employerName = await db.Employers.AsNoTracking()
                .Where(e => e.Id == empId)
                .Select(e => e.Name)
                .FirstOrDefaultAsync(ct);
        }

        if (eh.SiteId is int siteId)
        {
            var site = await db.Sites.AsNoTracking()
                .Where(s => s.Id == siteId)
                .Select(s => new { s.SiteCode, s.TimeZoneWindows })
                .FirstOrDefaultAsync(ct);

            if (site is not null)
            {
                siteCode = site.SiteCode;
                siteWindowsTz = site.TimeZoneWindows;
            }
        }

        if (eh.OrganizationId is int orgId)
            orgName = await db.Organizations.AsNoTracking()
                .Where(o => o.Id == orgId)
                .Select(o => o.Name)
                .FirstOrDefaultAsync(ct);

        if (eh.SubOrganizationId is int subId)
            subOrgName = await db.SubOrganizations.AsNoTracking()
                .Where(s => s.Id == subId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync(ct);

        if (eh.ManagerId is int manId)
            mgrName = await (
                from m in db.Managers.AsNoTracking()
                join e in db.Employees.AsNoTracking() on m.EmployeeId equals e.Id
                where m.Id == manId
                select (e.LastName ?? "") + ", " + (e.FirstName ?? "")
            ).FirstOrDefaultAsync(ct);

        if (eh.SupervisorId is int supId)
            supName = await (
                from s in db.Supervisors.AsNoTracking()
                join e in db.Employees.AsNoTracking() on s.EmployeeId equals e.Id
                where s.Id == supId
                select (e.LastName ?? "") + ", " + (e.FirstName ?? "")
            ).FirstOrDefaultAsync(ct);

        var prev = new PrevDetailsVm
        {
            EmployeeId = eh.EmployeeId,
            EffectiveDate = DateOnly.FromDateTime(eh.EffectiveDate),
            EmployerId = eh.EmployerId,
            SiteId = eh.SiteId,
            OrganizationId = eh.OrganizationId,
            SubOrganizationId = eh.SubOrganizationId,
            ManagerId = eh.ManagerId,
            SupervisorId = eh.SupervisorId,
            IsActive = eh.IsActive,
            IsLoa = eh.IsLoa,
            IsIntLoa = eh.IsIntLoa,
            IsRemote = eh.IsRemote,

            EmployerName = employerName,
            SiteCode = siteCode,
            OrganizationName = orgName,
            SubOrganizationName = subOrgName,
            ManagerName = mgrName,
            SupervisorName = supName
        };

        if (eh.ScheduleRequestId is int schReqId)
        {
            var sch = await db.AcrSchedules.AsNoTracking()
                .Where(s => s.AcrRequestId == schReqId && s.ShiftNumber == 1)
                .Select(s => new
                {
                    s.ShiftNumber,
                    s.MondayStart,
                    s.MondayEnd,
                    s.TuesdayStart,
                    s.TuesdayEnd,
                    s.WednesdayStart,
                    s.WednesdayEnd,
                    s.ThursdayStart,
                    s.ThursdayEnd,
                    s.FridayStart,
                    s.FridayEnd,
                    s.SaturdayStart,
                    s.SaturdayEnd,
                    s.SundayStart,
                    s.SundayEnd,
                    s.IsStaticBreakSchedule
                })
                .FirstOrDefaultAsync(ct);

            if (sch is not null)
            {
                prev.ShiftNumber = sch.ShiftNumber;
                prev.SchMon = SpanEt(sch.MondayStart, sch.MondayEnd, mode, siteWindowsTz);
                prev.SchTue = SpanEt(sch.TuesdayStart, sch.TuesdayEnd, mode, siteWindowsTz);
                prev.SchWed = SpanEt(sch.WednesdayStart, sch.WednesdayEnd, mode, siteWindowsTz);
                prev.SchThu = SpanEt(sch.ThursdayStart, sch.ThursdayEnd, mode, siteWindowsTz);
                prev.SchFri = SpanEt(sch.FridayStart, sch.FridayEnd, mode, siteWindowsTz);
                prev.SchSat = SpanEt(sch.SaturdayStart, sch.SaturdayEnd, mode, siteWindowsTz);
                prev.SchSun = SpanEt(sch.SundayStart, sch.SundayEnd, mode, siteWindowsTz);
                prev.IsStaticBreakSchedule = sch.IsStaticBreakSchedule;
            }
        }

        if (eh.OvertimeRequestId is int otReqId)
        {
            var ot = await db.AcrOvertimeSchedules.AsNoTracking()
                .Where(o => o.AcrRequestId == otReqId)
                .Select(o => new
                {
                    o.MondayTypeId,
                    o.TuesdayTypeId,
                    o.WednesdayTypeId,
                    o.ThursdayTypeId,
                    o.FridayTypeId,
                    o.SaturdayTypeId,
                    o.SundayTypeId
                })
                .FirstOrDefaultAsync(ct);

            if (ot is not null)
            {
                var typeMap = await GetOvertimeTypeMapAsync(db, ct);

                static string NameOrDash(Dictionary<int, string> map, int? id)
                    => id is int v && map.TryGetValue(v, out var n) ? n : "-";

                prev.OTMon = NameOrDash(typeMap, ot.MondayTypeId);
                prev.OTTue = NameOrDash(typeMap, ot.TuesdayTypeId);
                prev.OTWed = NameOrDash(typeMap, ot.WednesdayTypeId);
                prev.OTThu = NameOrDash(typeMap, ot.ThursdayTypeId);
                prev.OTFri = NameOrDash(typeMap, ot.FridayTypeId);
                prev.OTSat = NameOrDash(typeMap, ot.SaturdayTypeId);
                prev.OTSun = NameOrDash(typeMap, ot.SundayTypeId);
            }
        }

        return prev;
    }

    private static string Span(TimeOnly? start, TimeOnly? end)
        => (start is null && end is null)
            ? "-"
            : $"{(start?.ToString("HH:mm") ?? "--:--")} – {(end?.ToString("HH:mm") ?? "--:--")}";

    private static string SpanEt(
        TimeOnly? startEt,
        TimeOnly? endEt,
        TimeDisplayMode mode,
        string? siteWindowsTz)
    {
        var s = ConvertFromEt(startEt, mode, siteWindowsTz);
        var e = ConvertFromEt(endEt, mode, siteWindowsTz);
        return Span(s, e);
    }

    private static TimeOnly? ConvertFromEt(
        TimeOnly? et,
        TimeDisplayMode mode,
        string? siteWindowsTz)
    {
        if (et is null)
            return null;

        if (mode == TimeDisplayMode.Eastern)
            return et;

        var easternTz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        var targetTzId = mode switch
        {
            TimeDisplayMode.EmployeeLocal => string.IsNullOrWhiteSpace(siteWindowsTz)
                ? "Eastern Standard Time"
                : siteWindowsTz,
            TimeDisplayMode.Mountain => "Mountain Standard Time",
            _ => "Eastern Standard Time"
        };

        try
        {
            var targetTz = TimeZoneInfo.FindSystemTimeZoneById(targetTzId);
            var dtEt = new DateTime(2000, 1, 1, et.Value.Hour, et.Value.Minute, 0, DateTimeKind.Unspecified);
            var dtUtc = TimeZoneInfo.ConvertTimeToUtc(dtEt, easternTz);
            var dtLocal = TimeZoneInfo.ConvertTimeFromUtc(dtUtc, targetTz);

            return new TimeOnly(dtLocal.Hour, dtLocal.Minute);
        }
        catch
        {
            return et;
        }
    }

    private static async Task<Dictionary<int, string>> GetOvertimeTypeMapAsync(AomDbContext db, CancellationToken ct)
    {
        return await db.AcrOvertimeTypes
            .Select(t => new { t.Id, t.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
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