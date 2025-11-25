using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using System.Linq;

namespace MyApplication.Components.Service.Training
{
    // Simple row for lookup table
    public sealed record SkillLookupRow(
     int SkillId,
     int EmployeeId,
     string EmployeeName,
     string? SkillTypeName,
     DateOnly SkillDate
 );

    public sealed record SkillSearchVm(
        string? EmployeeSearch,
        int? SkillTypeId
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


    public interface ISkillService
    {
        Task AddSkillsBulkAsync(IEnumerable<SkillCreateDto> dtos, CancellationToken ct = default);
        Task<IReadOnlyList<SkillLookupRow>> SearchSkillsAsync(SkillSearchVm search, CancellationToken ct = default);
        Task<IReadOnlyList<SkillType>> GetSkillTypesAsync(bool activeOnly = true, CancellationToken ct = default);
        Task<IReadOnlyList<EmployeeOption>> SearchEmployeesAsync(string term, CancellationToken ct = default);
        Task AddSkillAsync(SkillCreateDto dto, CancellationToken ct = default);
        Task<IReadOnlyList<SkillEmployeeSummaryRow>> GetEmployeeSkillSummariesAsync(SkillSearchVm search, CancellationToken ct = default);

    }

    public sealed class SkillService : ISkillService
    {
        private readonly IDbContextFactory<AomDbContext> _dbFactory;

        public SkillService(IDbContextFactory<AomDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task AddSkillsBulkAsync(IEnumerable<SkillCreateDto> dtos, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            // Collect employee IDs and skill type IDs
            var empIds = dtos.Select(d => d.EmployeeId).Distinct().ToList();
            var skillIds = dtos.Select(d => d.SkillTypeId).Distinct().ToList();

            // Fetch existing combinations from DB
            var existingPairs = await db.Skills
                .Where(s => empIds.Contains(s.EmployeeId) &&
                            skillIds.Contains(s.SkillTypeId) &&
                            s.IsActive == true)
                .Select(s => new { s.EmployeeId, s.SkillTypeId })
                .ToListAsync(ct);

            var existingSet = existingPairs
                .Select(x => (x.EmployeeId, x.SkillTypeId))
                .ToHashSet();

            // Create only NEW skills (no duplicates)
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


        public async Task<IReadOnlyList<SkillLookupRow>> SearchSkillsAsync(SkillSearchVm search, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            var query = db.Skills
                .AsNoTracking()
                .Include(s => s.Employee)
                .Include(s => s.SkillType)
                .AsQueryable();

            // only active employees
            query = query.Where(s => s.Employee.IsActive == true);

            // only active skills (defensive)
            query = query.Where(s => s.IsActive == true);

            if (search.SkillTypeId is int skillTypeId)
            {
                query = query.Where(s => s.SkillTypeId == skillTypeId);
            }

            if (!string.IsNullOrWhiteSpace(search.EmployeeSearch))
            {
                var term = search.EmployeeSearch.Trim();

                query = query.Where(s =>
                    EF.Functions.Like(s.Employee.LastName, $"%{term}%") ||
                    EF.Functions.Like(s.Employee.FirstName, $"%{term}%") ||
                    EF.Functions.Like((s.Employee.LastName + ", " + s.Employee.FirstName), $"%{term}%")
                );
            }

            var rows = await query
                .OrderBy(s => s.Employee.LastName)
                .ThenBy(s => s.Employee.FirstName)
                .ThenByDescending(s => s.SkillDate)
                .Take(500)
                .Select(s => new SkillLookupRow(
                s.Id,
                s.EmployeeId,
                s.Employee.LastName + ", " + s.Employee.FirstName,
                s.SkillType.Name,
                s.SkillDate
))

                .ToListAsync(ct);

            return rows;
        }


        public async Task<IReadOnlyList<SkillType>> GetSkillTypesAsync(bool activeOnly = true, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            var query = db.SkillType.AsNoTracking().AsQueryable();
            if (activeOnly)
            {
                query = query.Where(st => st.IsActive == true);
            }

            return await query
                .OrderBy(st => st.Name)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<EmployeeOption>> SearchEmployeesAsync(string term, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.Employees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                q = q.Where(e =>
                    EF.Functions.Like(e.LastName, $"%{term}%") ||
                    EF.Functions.Like(e.FirstName, $"%{term}%")
                );
            }

            return await q
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Take(25)
                .Select(e => new EmployeeOption(
                    e.Id,
                    e.LastName + ", " + e.FirstName
                ))
                .ToListAsync(ct);
        }


        public async Task AddSkillAsync(SkillCreateDto dto, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            // Check if this skill already exists
            bool exists = await db.Skills
                .AnyAsync(s =>
                    s.EmployeeId == dto.EmployeeId &&
                    s.SkillTypeId == dto.SkillTypeId &&
                    s.IsActive == true,
                    ct);

            if (exists)
                return; // silently skip

            var entity = new Skills
            {
                EmployeeId = dto.EmployeeId,
                SkillTypeId = dto.SkillTypeId,
                SkillDate = dto.SkillDate,
                IsActive = true
            };

            db.Skills.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<SkillEmployeeSummaryRow>> GetEmployeeSkillSummariesAsync(
       SkillSearchVm search,
       CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            var query = db.Skills
                .AsNoTracking()
                .Include(s => s.Employee)
                .Include(s => s.SkillType)
                .AsQueryable();

            // active employees only
            query = query.Where(s => s.Employee.IsActive == true);

            // active skills only
            query = query.Where(s => s.IsActive == true);

            if (search.SkillTypeId is int skillTypeId)
            {
                query = query.Where(s => s.SkillTypeId == skillTypeId);
            }

            if (!string.IsNullOrWhiteSpace(search.EmployeeSearch))
            {
                var term = search.EmployeeSearch.Trim();

                query = query.Where(s =>
                    EF.Functions.Like(s.Employee.LastName, $"%{term}%") ||
                    EF.Functions.Like(s.Employee.FirstName, $"%{term}%") ||
                    EF.Functions.Like(s.Employee.LastName + ", " + s.Employee.FirstName, $"%{term}%")
                );
            }

            // 1) Flatten to a shape EF can translate
            var raw = await query
                .Select(s => new
                {
                    s.EmployeeId,
                    s.Employee.LastName,
                    s.Employee.FirstName,
                    s.SkillTypeId,
                    SkillName = s.SkillType.Name,
                    s.SkillDate
                })
                .ToListAsync(ct);   // everything below is LINQ-to-Objects

            // 2) Group & aggregate in memory
            var grouped = raw
                .GroupBy(x => new { x.EmployeeId, x.LastName, x.FirstName })
                .Select(g =>
                {
                    // build distinct skill list per employee
                    var skillItems = g
                        .GroupBy(x => x.SkillTypeId)
                        .Select(grp => new SkillSummaryItem(
                            grp.Key,
                            grp.Select(z => z.SkillName).First()
                        ))
                        .OrderBy(x => x.SkillName)
                        .ToList();

                    var effectiveDate = g.Max(x => x.SkillDate);

                    return new SkillEmployeeSummaryRow(
                        g.Key.EmployeeId,
                        $"{g.Key.LastName}, {g.Key.FirstName}",
                        skillItems,
                        effectiveDate
                    );
                })
                .OrderBy(r => r.EmployeeName)
                .ToList();

            return grouped;
        }



    }
}
