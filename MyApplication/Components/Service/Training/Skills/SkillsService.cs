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
        int? SkillTypeId,
        int? ManagerId,    // <--- Added
        int? SupervisorId  // <--- Added
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
        Task<IReadOnlyList<SkillLookupRow>> SearchSkillsAsync(SkillSearchVm search, CancellationToken ct = default);
        Task<IReadOnlyList<SkillType>> GetSkillTypesAsync(bool activeOnly = true, CancellationToken ct = default);
        Task<IReadOnlyList<EmployeeOption>> SearchEmployeesAsync(string term, CancellationToken ct = default);
        Task AddSkillAsync(SkillCreateDto dto, CancellationToken ct = default);
        Task<IReadOnlyList<SkillEmployeeSummaryRow>> GetEmployeeSkillSummariesAsync(SkillSearchVm search, CancellationToken ct = default);

        // Lookup Methods
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
                // Join for Hierarchy Filtering
                .Join(db.EmployeeCurrentDetails,
                      s => s.EmployeeId,
                      d => d.EmployeeId,
                      (s, d) => new { Skill = s, Details = d })
                .AsQueryable();

            // Active checks
            query = query.Where(x => x.Skill.Employee.IsActive == true && x.Skill.IsActive == true);

            // Hierarchy Filters
            if (search.ManagerId.HasValue)
                query = query.Where(x => x.Details.ManagerId == search.ManagerId);

            if (search.SupervisorId.HasValue)
                query = query.Where(x => x.Details.SupervisorId == search.SupervisorId);

            // Skill Type Filter
            if (search.SkillTypeId is int skillTypeId)
            {
                query = query.Where(x => x.Skill.SkillTypeId == skillTypeId);
            }

            // Name Search
            if (!string.IsNullOrWhiteSpace(search.EmployeeSearch))
            {
                var term = search.EmployeeSearch.Trim();
                query = query.Where(x =>
                    EF.Functions.Like(x.Skill.Employee.LastName, $"%{term}%") ||
                    EF.Functions.Like(x.Skill.Employee.FirstName, $"%{term}%") ||
                    EF.Functions.Like((x.Skill.Employee.LastName + ", " + x.Skill.Employee.FirstName), $"%{term}%")
                );
            }

            var rows = await query
                .OrderBy(x => x.Skill.Employee.LastName)
                .ThenBy(x => x.Skill.Employee.FirstName)
                .ThenByDescending(x => x.Skill.SkillDate)
                .Take(500)
                .Select(x => new SkillLookupRow(
                    x.Skill.Id,
                    x.Skill.EmployeeId,
                    x.Skill.Employee.LastName + ", " + x.Skill.Employee.FirstName + (x.Skill.Employee.MiddleInitial == null ? "" : " " + x.Skill.Employee.MiddleInitial),
                    x.Skill.SkillType.Name,
                    x.Skill.SkillDate
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

        // --- NEW: Safe Lookup Methods ---
        public async Task<List<KeyValuePair<int, string>>> GetManagersAsync(CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            // 1. Fetch raw data
            var data = await (
                from m in db.Managers.AsNoTracking()
                join e in db.Employees.AsNoTracking() on m.EmployeeId equals e.Id
                where m.IsActive == true && e.IsActive == true
                orderby e.LastName, e.FirstName
                select new
                {
                    m.Id,
                    e.LastName,
                    e.FirstName,
                    e.MiddleInitial
                }
            ).ToListAsync(ct);

            // 2. Format in memory
            return data.Select(x => new KeyValuePair<int, string>(
                x.Id,
                $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"
            )).ToList();
        }

        public async Task<List<KeyValuePair<int, string>>> GetSupervisorsAsync(CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            var data = await (
                from s in db.Supervisors.AsNoTracking()
                join e in db.Employees.AsNoTracking() on s.EmployeeId equals e.Id
                where s.IsActive == true && e.IsActive == true
                orderby e.LastName, e.FirstName
                select new
                {
                    s.Id,
                    e.LastName,
                    e.FirstName,
                    e.MiddleInitial
                }
            ).ToListAsync(ct);

            return data.Select(x => new KeyValuePair<int, string>(
                x.Id,
                $"{x.LastName}, {x.FirstName}{(!string.IsNullOrEmpty(x.MiddleInitial) ? " " + x.MiddleInitial : "")}"
            )).ToList();
        }

        public async Task<IReadOnlyList<EmployeeOption>> SearchEmployeesAsync(string term, CancellationToken ct = default)
        {
            using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.Employees.AsNoTracking().Where(e => e.IsActive == true);

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
                    e.LastName + ", " + e.FirstName + (e.MiddleInitial == null ? "" : " " + e.MiddleInitial)
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
                .Join(db.EmployeeCurrentDetails,
                      s => s.EmployeeId,
                      d => d.EmployeeId,
                      (s, d) => new { Skill = s, Details = d })
                .AsQueryable();

            query = query.Where(x => x.Skill.Employee.IsActive == true && x.Skill.IsActive == true);

            // Hierarchy Filters
            if (search.ManagerId.HasValue)
                query = query.Where(x => x.Details.ManagerId == search.ManagerId);

            if (search.SupervisorId.HasValue)
                query = query.Where(x => x.Details.SupervisorId == search.SupervisorId);

            if (search.SkillTypeId is int skillTypeId)
            {
                query = query.Where(x => x.Skill.SkillTypeId == skillTypeId);
            }

            if (!string.IsNullOrWhiteSpace(search.EmployeeSearch))
            {
                var term = search.EmployeeSearch.Trim();
                query = query.Where(x =>
                    EF.Functions.Like(x.Skill.Employee.LastName, $"%{term}%") ||
                    EF.Functions.Like(x.Skill.Employee.FirstName, $"%{term}%") ||
                    EF.Functions.Like(x.Skill.Employee.LastName + ", " + x.Skill.Employee.FirstName, $"%{term}%")
                );
            }

            // 1) Flatten
            var raw = await query
                .Select(x => new
                {
                    x.Skill.EmployeeId,
                    x.Skill.Employee.LastName,
                    x.Skill.Employee.FirstName,
                    x.Skill.Employee.MiddleInitial,
                    x.Skill.SkillTypeId,
                    SkillName = x.Skill.SkillType.Name,
                    x.Skill.SkillDate
                })
                .ToListAsync(ct);

            // 2) Group & aggregate in memory
            var grouped = raw
                .GroupBy(x => new { x.EmployeeId, x.LastName, x.FirstName, x.MiddleInitial })
                .Select(g =>
                {
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
                        $"{g.Key.LastName}, {g.Key.FirstName}{(!string.IsNullOrEmpty(g.Key.MiddleInitial) ? " " + g.Key.MiddleInitial : "")}",
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