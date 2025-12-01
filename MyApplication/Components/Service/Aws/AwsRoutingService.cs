using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Aws;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Aws
{
    public class AwsRoutingService
    {
        private readonly IDbContextFactory<AomDbContext> _factory;

        public AwsRoutingService(IDbContextFactory<AomDbContext> factory)
        {
            _factory = factory;
        }

        // =========================
        // CALL QUEUES
        // =========================
        public async Task<List<CallQueue>> GetQueuesAsync()
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.CallQueues
                .Include(q => q.SkillType) // <--- CHANGED: Include the Type directly
                .OrderBy(q => q.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SaveQueueAsync(CallQueue queue)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            if (queue.Id == 0)
            {
                ctx.CallQueues.Add(queue);
            }
            else
            {
                ctx.CallQueues.Update(queue);
            }
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteQueueAsync(int id)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var q = await ctx.CallQueues.FindAsync(id);
            if (q != null)
            {
                ctx.CallQueues.Remove(q);
                await ctx.SaveChangesAsync();
            }
        }

        // =========================
        // ROUTING PROFILES
        // =========================
        public async Task<List<RoutingProfile>> GetProfilesAsync()
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.RoutingProfiles
                .Include(p => p.RoutingProfileQueues)        // <--- Load the Mapping
                .ThenInclude(rpq => rpq.CallQueue)           // <--- Load the Queue Details
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<RoutingProfile?> GetProfileDetailsAsync(int id)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.RoutingProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task SaveProfileAsync(RoutingProfile profile)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            if (profile.Id == 0)
            {
                ctx.RoutingProfiles.Add(profile);
            }
            else
            {
                ctx.RoutingProfiles.Update(profile);
            }
            await ctx.SaveChangesAsync();
        }

        // =========================
        // PROFILE <-> QUEUE MAPPING
        // =========================
        public async Task<List<RoutingProfileQueue>> GetQueuesForProfileAsync(int profileId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.RoutingProfileQueues
                .Include(x => x.CallQueue)
                .Where(x => x.RoutingProfileId == profileId)
                .OrderBy(x => x.Priority)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddQueueToProfileAsync(RoutingProfileQueue mapping)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            ctx.RoutingProfileQueues.Add(mapping);
            await ctx.SaveChangesAsync();
        }

        public async Task RemoveQueueFromProfileAsync(int mappingId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var item = await ctx.RoutingProfileQueues.FindAsync(mappingId);
            if (item != null)
            {
                ctx.RoutingProfileQueues.Remove(item);
                await ctx.SaveChangesAsync();
            }
        }

        // =========================
        // EMPLOYEE ASSIGNMENT
        // =========================
        public async Task AssignProfilesToEmployeesAsync(IEnumerable<int> employeeIds, int? weekdayProfileId, int? weekendProfileId)
        {
            using var ctx = await _factory.CreateDbContextAsync();

            // 1. Fetch existing assignments for these employees
            var existingAssignments = await ctx.EmployeeRoutingProfiles
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToListAsync();

            // 2. Process each selected employee
            foreach (var empId in employeeIds)
            {
                var assignment = existingAssignments.FirstOrDefault(x => x.EmployeeId == empId);

                if (assignment != null)
                {
                    // Update existing
                    assignment.WeekdayProfileId = weekdayProfileId;
                    assignment.WeekendProfileId = weekendProfileId;
                }
                else
                {
                    // Create new
                    ctx.EmployeeRoutingProfiles.Add(new EmployeeRoutingProfile
                    {
                        EmployeeId = empId,
                        WeekdayProfileId = weekdayProfileId,
                        WeekendProfileId = weekendProfileId
                    });
                }
            }

            await ctx.SaveChangesAsync();
        }
        public async Task<EmployeeRoutingProfile?> GetEmployeeAssignmentAsync(int employeeId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.EmployeeRoutingProfiles
                .Include(x => x.WeekdayProfile)
                .Include(x => x.WeekendProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
        }

        public async Task SaveEmployeeAssignmentAsync(EmployeeRoutingProfile assignment)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            if (assignment.Id == 0)
            {
                ctx.EmployeeRoutingProfiles.Add(assignment);
            }
            else
            {
                ctx.EmployeeRoutingProfiles.Update(assignment);
            }
            await ctx.SaveChangesAsync();
        }

        // Helper to get Skills for the Queue Dropdown
        public async Task<List<SkillType>> GetSkillTypesAsync()
        {
            using var ctx = await _factory.CreateDbContextAsync();

          
            return await ctx.SkillType
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}