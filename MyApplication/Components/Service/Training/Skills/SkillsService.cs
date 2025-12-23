using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using System.Linq.Expressions;

namespace MyApplication.Components.Service.Training
{
    // --- Data Transfer Objects ---
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int TotalCount
    );

    public sealed record SkillLookupRow(
      int SkillId,
      int EmployeeId,
      string EmployeeName,
      string? SkillTypeName,
      DateOnly SkillDate
    );

    public sealed record SkillSearchVm(
        string? EmployeeSearch,
        int? SkillTypeId,
        int? ManagerId,
        int? SupervisorId
    );

    public sealed record SkillEmployeeSummaryRow(
        int EmployeeId,
        string EmployeeName,
        IReadOnlyList<SkillSummaryItem> Skills,
        DateOnly EffectiveDate
    );

    public sealed record SkillSummaryItem(
        int SkillTypeId,
        string SkillName
    );

    public sealed record EmployeeOption(
        int Id,
        string Display
    );

    public sealed record SkillCreateDto(
        int EmployeeId,
        int SkillTypeId,
        DateOnly SkillDate
    );

    public interface ISkillsService
    {
        Task AddSkillsBulkAsync(IEnumerable<SkillCreateDto> dtos, CancellationToken ct = default);

        // Updated to support Paging/Sorting
        Task<PagedResult<SkillLookupRow>> SearchSkillsServerDataAsync(SkillSearchVm search, int pageIndex, int pageSize, string? sortLabel, int sortDirection, CancellationToken ct = default);
        Task<PagedResult<SkillEmployeeSummaryRow>> GetEmployeeSkillSummariesServerDataAsync(SkillSearchVm search, int pageIndex, int pageSize, string? sortLabel, int sortDirection, CancellationToken ct = default);

        Task<IReadOnlyList<SkillType>> GetSkillTypesAsync(bool activeOnly = true, CancellationToken ct = default);
        Task<IReadOnlyList<EmployeeOption>> SearchEmployeesAsync(string term, CancellationToken ct = default);
        Task AddSkillAsync(SkillCreateDto dto, CancellationToken ct = default);

        Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default);
        Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default);
    }

    public sealed class SkillsService : ISkillsService
    {
        private readonly IDbContextFactory<AomDbContext> _dbFactory;

        public SkillsService(IDbContextFactory<AomDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task AddSkillsBulkAsync(IEnumerable<SkillCreateDto> dtos, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);
            var empIds = dtos.Select(d => d.EmployeeId).Distinct().ToList();
            var skillIds = dtos.Select(d => d.SkillTypeId).Distinct().ToList();

            var existingPairs = await db.Skills
                .Where(s => empIds.Contains(s.EmployeeId) &&
                            skillIds.Contains(s.SkillTypeId) &&
                            s.IsActive == true)
                .Select(s => new { s.EmployeeId, s.SkillTypeId })
                .ToListAsync(ct);

            var existingSet = existingPairs.Select(x => (x.EmployeeId, x.SkillTypeId)).ToHashSet();

            var toInsert = dtos
                .Where(d => !existingSet.Contains((d.EmployeeId, d.SkillTypeId)))
                .Select(d => new Skills
                {
                    EmployeeId = d.EmployeeId,
                    SkillTypeId = d.SkillTypeId,
                    SkillDate = d.SkillDate,
                    IsActive = true
                })
                .ToList();

            if (toInsert.Count > 0)
            {
                db.Skills.AddRange(toInsert);
                await db.SaveChangesAsync(ct);
            }
        }

        public async Task AddSkillAsync(SkillCreateDto dto, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);
            bool exists = await db.Skills.AnyAsync(s => s.EmployeeId == dto.EmployeeId && s.SkillTypeId == dto.SkillTypeId && s.IsActive == true, ct);
            if (exists) return;

            db.Skills.Add(new Skills
            {
                EmployeeId = dto.EmployeeId,
                SkillTypeId = dto.SkillTypeId,
                SkillDate = dto.SkillDate,
                IsActive = true
            });
            await db.SaveChangesAsync(ct);
        }

        // --- NEW: Server Side Search for Line Items ---
        public async Task<PagedResult<SkillLookupRow>> SearchSkillsServerDataAsync(
            SkillSearchVm search,
            int pageIndex,
            int pageSize,
            string? sortLabel,
            int sortDirection, // 0: None, 1: Asc, 2: Desc (MudBlazor Enums)
            CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            var query = db.Skills
                .AsNoTracking()
                .Include(s => s.Employee)
                .Include(s => s.SkillType)
                .Join(db.EmployeeCurrentDetails,
                      s => s.EmployeeId,
                      d => d.EmployeeId,
                      (s, d) => new { Skill = s, Details = d })
                .Where(x => x.Skill.Employee.IsActive == true && x.Skill.IsActive == true);

            // 1. Apply Filters
            if (search.ManagerId.HasValue) query = query.Where(x => x.Details.ManagerId == search.ManagerId);
            if (search.SupervisorId.HasValue) query = query.Where(x => x.Details.SupervisorId == search.SupervisorId);
            if (search.SkillTypeId.HasValue) query = query.Where(x => x.Skill.SkillTypeId == search.SkillTypeId);

            if (!string.IsNullOrWhiteSpace(search.EmployeeSearch))
            {
                var term = search.EmployeeSearch.Trim();
                query = query.Where(x =>
                    EF.Functions.Like(x.Skill.Employee.LastName, $"%{term}%") ||
                    EF.Functions.Like(x.Skill.Employee.FirstName, $"%{term}%")
                );
            }

            // 2. Count before paging
            var totalCount = await query.CountAsync(ct);

            // 3. Apply Sorting
            // MudBlazor SortDirection: 1 = Ascending, 2 = Descending
            bool isAsc = sortDirection != 2;

            switch (sortLabel)
            {
                case "SkillTypeName":
                    query = isAsc ? query.OrderBy(x => x.Skill.SkillType.Name) : query.OrderByDescending(x => x.Skill.SkillType.Name);
                    break;
                case "SkillDate":
                    query = isAsc ? query.OrderBy(x => x.Skill.SkillDate) : query.OrderByDescending(x => x.Skill.SkillDate);
                    break;
                case "EmployeeName":
                default:
                    query = isAsc
                        ? query.OrderBy(x => x.Skill.Employee.LastName).ThenBy(x => x.Skill.Employee.FirstName)
                        : query.OrderByDescending(x => x.Skill.Employee.LastName).ThenByDescending(x => x.Skill.Employee.FirstName);
                    break;
            }

            // 4. Page and Fetch
            var items = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(x => new SkillLookupRow(
                    x.Skill.Id,
                    x.Skill.EmployeeId,
                    x.Skill.Employee.LastName + ", " + x.Skill.Employee.FirstName + (x.Skill.Employee.MiddleInitial == null ? "" : " " + x.Skill.Employee.MiddleInitial),
                    x.Skill.SkillType.Name,
                    x.Skill.SkillDate
                ))
                .ToListAsync(ct);

            return new PagedResult<SkillLookupRow>(items, totalCount);
        }

        // --- NEW: Server Side Search for Summary (Grouped) ---
        public async Task<PagedResult<SkillEmployeeSummaryRow>> GetEmployeeSkillSummariesServerDataAsync(
            SkillSearchVm search,
            int pageIndex,
            int pageSize,
            string? sortLabel,
            int sortDirection,
            CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            // Base query to find matching Employees
            var baseQuery = db.Skills
                .AsNoTracking()
                .Join(db.EmployeeCurrentDetails, s => s.EmployeeId, d => d.EmployeeId, (s, d) => new { Skill = s, Details = d })
                .Where(x => x.Skill.Employee.IsActive == true && x.Skill.IsActive == true);

            if (search.ManagerId.HasValue) baseQuery = baseQuery.Where(x => x.Details.ManagerId == search.ManagerId);
            if (search.SupervisorId.HasValue) baseQuery = baseQuery.Where(x => x.Details.SupervisorId == search.SupervisorId);
            if (search.SkillTypeId.HasValue) baseQuery = baseQuery.Where(x => x.Skill.SkillTypeId == search.SkillTypeId);
            if (!string.IsNullOrWhiteSpace(search.EmployeeSearch))
            {
                var term = search.EmployeeSearch.Trim();
                baseQuery = baseQuery.Where(x => EF.Functions.Like(x.Skill.Employee.LastName, $"%{term}%") || EF.Functions.Like(x.Skill.Employee.FirstName, $"%{term}%"));
            }

            // 1. Group by Employee first to handle paging on "Rows" (Employees), not lines
            var groupedQuery = baseQuery
                .GroupBy(x => x.Skill.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    // We need these for sorting
                    LastName = g.Max(x => x.Skill.Employee.LastName),
                    FirstName = g.Max(x => x.Skill.Employee.FirstName),
                    MiddleInitial = g.Max(x => x.Skill.Employee.MiddleInitial),
                    MaxDate = g.Max(x => x.Skill.SkillDate)
                });

            var totalCount = await groupedQuery.CountAsync(ct);

            // 2. Sort the Employees
            bool isAsc = sortDirection != 2;
            if (sortLabel == "EffectiveDate")
            {
                groupedQuery = isAsc ? groupedQuery.OrderBy(x => x.MaxDate) : groupedQuery.OrderByDescending(x => x.MaxDate);
            }
            else
            {
                // Default to Name
                groupedQuery = isAsc
                    ? groupedQuery.OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                    : groupedQuery.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName);
            }

            // 3. Get the Page of Employee IDs
            var pagedEmployees = await groupedQuery
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var empIds = pagedEmployees.Select(x => x.EmployeeId).ToList();

            // 4. Fetch Details for these specific IDs
            // We fetch the skills again just for this page
            var rawSkills = await db.Skills
                .AsNoTracking()
                .Include(s => s.SkillType)
                .Where(s => empIds.Contains(s.EmployeeId) && s.IsActive == true)
                .Select(s => new
                {
                    s.EmployeeId,
                    s.SkillTypeId,
                    SkillName = s.SkillType.Name,
                    s.SkillDate
                })
                .ToListAsync(ct);

            // 5. Construct the Result in Memory
            var resultItems = new List<SkillEmployeeSummaryRow>();

            // We iterate over 'pagedEmployees' to maintain the sort order we defined in step 2
            foreach (var emp in pagedEmployees)
            {
                var empSkills = rawSkills.Where(r => r.EmployeeId == emp.EmployeeId).ToList();

                var skillItems = empSkills
                    .GroupBy(x => x.SkillTypeId)
                    .Select(g => new SkillSummaryItem(g.Key, g.First().SkillName)) // Name should be consistent
                    .OrderBy(x => x.SkillName)
                    .ToList();

                resultItems.Add(new SkillEmployeeSummaryRow(
                    emp.EmployeeId,
                    $"{emp.LastName}, {emp.FirstName}{(!string.IsNullOrEmpty(emp.MiddleInitial) ? " " + emp.MiddleInitial : "")}",
                    skillItems,
                    emp.MaxDate // Use the one we calculated in the group query for consistency
                ));
            }

            return new PagedResult<SkillEmployeeSummaryRow>(resultItems, totalCount);
        }

        // --- Existing Lookups ---
        public async Task<IReadOnlyList<SkillType>> GetSkillTypesAsync(bool activeOnly = true, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);
            var query = db.SkillType.AsNoTracking().AsQueryable();
            if (activeOnly) query = query.Where(st => st.IsActive == true);
            return await query.OrderBy(st => st.Name).ToListAsync(ct);
        }

        public async Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await (from m in db.Managers.AsNoTracking()
                          join e in db.Employees.AsNoTracking() on m.EmployeeId equals e.Id
                          where m.IsActive == true && e.IsActive == true
                          orderby e.LastName, e.FirstName
                          select new KeyValuePair<int, string>(m.Id, $"{e.LastName}, {e.FirstName}"))
                          .ToListAsync(ct);
        }

        public async Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await (from s in db.Supervisors.AsNoTracking()
                          join e in db.Employees.AsNoTracking() on s.EmployeeId equals e.Id
                          where s.IsActive == true && e.IsActive == true
                          orderby e.LastName, e.FirstName
                          select new KeyValuePair<int, string>(s.Id, $"{e.LastName}, {e.FirstName}"))
                          .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<EmployeeOption>> SearchEmployeesAsync(string term, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);
            var q = db.Employees.AsNoTracking().Where(e => e.IsActive == true);
            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                q = q.Where(e => EF.Functions.Like(e.LastName, $"%{term}%") || EF.Functions.Like(e.FirstName, $"%{term}%"));
            }
            return await q.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).Take(25)
                .Select(e => new EmployeeOption(e.Id, $"{e.LastName}, {e.FirstName}"))
                .ToListAsync(ct);
        }
    }
}