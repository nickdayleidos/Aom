using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Security;

namespace MyApplication.Components.Service.Security
{
    // DTO for the Index Table
    public class RoleAssignmentDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string? DisplayName { get; set; } // The friendly name
    }

    // DTO for the Autocomplete Search
    public class EmployeeUserDto
    {
        public string Login { get; set; }
        public string FullName { get; set; }

        // Helper for the UI to show "Day, Nick (dayng)"
        public override string ToString() => $"{FullName} ({Login})";
    }

    public class SecurityService
    {
        private readonly IDbContextFactory<AomDbContext> _factory;

        public SecurityService(IDbContextFactory<AomDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<AppRole>> GetRolesAsync()
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.AppRoles.OrderBy(r => r.Name).ToListAsync();
        }

        // UPDATED: Now returns DTOs with mapped Employee Names
        public async Task<List<RoleAssignmentDto>> GetAssignmentsWithDetailsAsync()
        {
            using var ctx = await _factory.CreateDbContextAsync();

            var assignments = await ctx.AppRoleAssignments
                .Include(a => a.AppRole)
                .AsNoTracking()
                .ToListAsync();

            // 1. Get all unique user logins from the assignments
            var userLogins = assignments
                .Where(x => x.Type == "User")
                .Select(x => x.Identifier)
                .Distinct()
                .ToList();

            // 2. Fetch matching Employees
            var employees = await ctx.Employees
                .Where(e => userLogins.Contains(e.DomainLoginName))
                .Select(e => new { e.DomainLoginName, Name = e.LastName + ", " + e.FirstName })
                .ToListAsync();

            var empDict = employees.ToDictionary(e => e.DomainLoginName, e => e.Name, StringComparer.OrdinalIgnoreCase);

            // 3. Map to DTO
            return assignments.Select(a => new RoleAssignmentDto
            {
                Id = a.Id,
                RoleName = a.AppRole?.Name ?? "Unknown",
                Type = a.Type,
                Identifier = a.Identifier,
                // If found in DB, show Name. If not (or if Group), show Identifier.
                DisplayName = (a.Type == "User" && empDict.ContainsKey(a.Identifier))
                              ? empDict[a.Identifier]
                              : a.Identifier
            }).OrderBy(x => x.RoleName).ThenBy(x => x.Type).ToList();
        }

        public async Task AddAssignmentAsync(AppRoleAssignment assignment)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            ctx.AppRoleAssignments.Add(assignment);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAssignmentAsync(int id)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var item = await ctx.AppRoleAssignments.FindAsync(id);
            if (item != null)
            {
                ctx.AppRoleAssignments.Remove(item);
                await ctx.SaveChangesAsync();
            }
        }

        // UPDATED: Search now returns objects with Names
        public async Task<List<EmployeeUserDto>> SearchEmployeesAsync(string query)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Employees
                .Where(e => (e.DomainLoginName != null && e.DomainLoginName.Contains(query)) ||
                            (e.LastName.Contains(query)) ||
                            (e.FirstName.Contains(query)))
                .Take(20)
                .Select(e => new EmployeeUserDto
                {
                    Login = e.DomainLoginName,
                    FullName = e.LastName + ", " + e.FirstName
                })
                .ToListAsync();
        }
    }
}