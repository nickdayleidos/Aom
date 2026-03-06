using Microsoft.EntityFrameworkCore;
using MyApplication.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service.Employee;

public sealed class EmployeeListService
{
    private readonly IDbContextFactory<AomDbContext> _contextFactory;

    public EmployeeListService(IDbContextFactory<AomDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<EmployeeDto>> SearchAsync(string searchText, bool activeOnly = true)
    {
        return await GetEmployeesAsync(new EmployeesFilterDto { SearchText = searchText, IncludeInactive = !activeOnly });
    }

    public async Task<List<EmployeeListItem>> SearchAsync(string searchText, bool activeOnly, object? mode, int take, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // Construct query using EmployeeHistory to find the latest record for each employee
        var latestHistoryIds = context.EmployeeHistory
            .GroupBy(h => h.EmployeeId)
            .Select(g => g.OrderByDescending(x => x.EffectiveDate).Select(x => x.Id).FirstOrDefault());

        var query = context.EmployeeHistory
            .Where(h => latestHistoryIds.Contains(h.Id))
            .Include(x => x.Employee)
            .Include(x => x.Organization)
            .Include(x => x.SubOrganization)
            .Include(x => x.Supervisor).ThenInclude(s => s!.Employee)
            .Include(x => x.Manager).ThenInclude(m => m!.Employee)
            .Include(x => x.ScheduleRequest).ThenInclude(s => s!.AcrSchedules)
            .AsQueryable();

        if (activeOnly)
        {
            query = query.Where(x => x.Employee!.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var terms = searchText.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                query = query.Where(x => x.Employee!.FirstName!.Contains(term) ||
                                         x.Employee!.LastName!.Contains(term) ||
                                         (x.Employee!.MiddleInitial != null && x.Employee!.MiddleInitial.Contains(term)) ||
                                         x.Employee!.Id.ToString().Contains(term));
            }
        }

        if (take > 0)
        {
            query = query.Take(take);
        }

        var items = await query.ToListAsync(ct); // Materialize to fetch related entities efficiently

        var empIds = items.Select(x => x.EmployeeId).ToList();
        var routingProfiles = await context.EmployeeRoutingProfiles
            .Where(rp => empIds.Contains(rp.EmployeeId))
            .Include(rp => rp.WeekdayProfile)
            .Include(rp => rp.WeekendProfile)
            .ToListAsync(ct);

        var skills = await context.Skills
            .Where(s => empIds.Contains(s.EmployeeId) && s.IsActive == true)
            .Include(s => s.SkillType)
            .ToListAsync(ct);

        var result = new List<EmployeeListItem>();

        foreach (var h in items)
        {
            var rp = routingProfiles.FirstOrDefault(p => p.EmployeeId == h.EmployeeId);
            var empSkills = skills.Where(s => s.EmployeeId == h.EmployeeId).Select(s => s.SkillType.Name).ToList();

            string scheduleStr = "-";
            if (h.ScheduleRequest != null && h.ScheduleRequest.AcrSchedules != null)
            {
                // Assuming mode is passed as TimeDisplayMode, let's cast it or default to Eastern
                var timeMode = mode is TimeDisplayMode tm ? tm : TimeDisplayMode.Eastern;
                scheduleStr = ScheduleFormatHelper.FormatSchedule(h.ScheduleRequest.AcrSchedules.FirstOrDefault(s => s.ShiftNumber == 1), timeMode);
            }

            result.Add(new EmployeeListItem
            {
                Id = h.EmployeeId,
                FirstName = h.Employee!.FirstName,
                LastName = h.Employee!.LastName,
                MiddleInitial = h.Employee!.MiddleInitial,
                IsActive = h.Employee!.IsActive,
                Organization = h.Organization != null ? h.Organization.Name : null,
                SubOrganization = h.SubOrganization != null ? h.SubOrganization.Name : null,
                Supervisor = h.Supervisor != null ? $"{h.Supervisor.Employee!.LastName}, {h.Supervisor.Employee!.FirstName}" : null,
                Manager = h.Manager != null ? $"{h.Manager.Employee!.LastName}, {h.Manager.Employee!.FirstName}" : null,
                Skills = empSkills,
                WeekdayProfile = rp?.WeekdayProfile?.Name,
                WeekendProfile = rp?.WeekendProfile?.Name,
                Schedule = scheduleStr
            });
        }

        return result;
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync(EmployeesFilterDto filter)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var latestHistoryIds = context.EmployeeHistory
            .GroupBy(h => h.EmployeeId)
            .Select(g => g.OrderByDescending(x => x.EffectiveDate).Select(x => x.Id).FirstOrDefault());

        var query = context.EmployeeHistory
            .Where(h => latestHistoryIds.Contains(h.Id))
            .Include(x => x.Employee)
            .Include(x => x.Organization)
            .Include(x => x.SubOrganization)
            .Include(x => x.Site)
            .Include(x => x.Supervisor).ThenInclude(s => s!.Employee)
            .Include(x => x.Manager).ThenInclude(m => m!.Employee)
            .AsQueryable();

        if (!filter.IncludeInactive)
        {
            query = query.Where(x => x.Employee!.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var terms = filter.SearchText.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                query = query.Where(x => x.Employee!.FirstName!.Contains(term) ||
                                         x.Employee!.LastName!.Contains(term) ||
                                         (x.Employee!.MiddleInitial != null && x.Employee!.MiddleInitial.Contains(term)) ||
                                         x.Employee!.Id.ToString().Contains(term));
            }
        }

        if (filter.OrganizationIds != null && filter.OrganizationIds.Any())
        {
            query = query.Where(x => x.OrganizationId.HasValue && filter.OrganizationIds.Contains(x.OrganizationId.Value));
        }

        if (filter.SubOrganizationIds != null && filter.SubOrganizationIds.Any())
        {
            query = query.Where(x => x.SubOrganizationId.HasValue && filter.SubOrganizationIds.Contains(x.SubOrganizationId.Value));
        }

        if (filter.SiteIds != null && filter.SiteIds.Any())
        {
            query = query.Where(x => x.SiteId.HasValue && filter.SiteIds.Contains(x.SiteId.Value));
        }

        if (filter.SupervisorIds != null && filter.SupervisorIds.Any())
        {
            query = query.Where(x => x.SupervisorId.HasValue && filter.SupervisorIds.Contains(x.SupervisorId.Value));
        }

        if (filter.ManagerIds != null && filter.ManagerIds.Any())
        {
            query = query.Where(x => x.ManagerId.HasValue && filter.ManagerIds.Contains(x.ManagerId.Value));
        }

        var items = await query.ToListAsync();
        var empIds = items.Select(x => x.EmployeeId).ToList();
        var skills = await context.Skills
            .Where(s => empIds.Contains(s.EmployeeId) && s.IsActive == true)
            .Include(s => s.SkillType)
            .ToListAsync();

        return items.Select(x =>
        {
            var fullName = $"{x.Employee!.LastName}, {x.Employee!.FirstName}";
            if (!string.IsNullOrEmpty(x.Employee!.MiddleInitial)) fullName += $" {x.Employee!.MiddleInitial}";
            fullName += $" ({x.Employee!.Id})";

            return new EmployeeDto
            {
                Id = x.EmployeeId,
                Name = fullName,
                Organization = x.Organization?.Name,
                OrganizationId = x.OrganizationId ?? 0,
                SubOrganization = x.SubOrganization?.Name,
                SubOrganizationId = x.SubOrganizationId ?? 0,
                Site = x.Site?.SiteCode,
                SiteId = x.SiteId ?? 0,
                Supervisor = x.Supervisor != null ? $"{x.Supervisor.Employee!.LastName}, {x.Supervisor.Employee!.FirstName}" : null,
                SupervisorId = x.SupervisorId,
                Manager = x.Manager != null ? $"{x.Manager.Employee!.LastName}, {x.Manager.Employee!.FirstName}" : null,
                ManagerId = x.ManagerId,
                IsActive = x.Employee!.IsActive,

                Skills = skills.Where(s => s.EmployeeId == x.EmployeeId).Select(s => s.SkillType!.Name).ToList()
            };
        }).ToList();
    }

    public async Task<List<OrganizationDto>> GetOrganizationsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Organizations
            .Where(x => x.IsActive == true)
            .Select(x => new OrganizationDto { Id = x.Id, Name = x.Name! })
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<SubOrganizationDto>> GetSubOrganizationsAsync(IEnumerable<int>? organizationIds = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.SubOrganizations
            .Where(x => x.IsActive == true);

        if (organizationIds != null && organizationIds.Any())
        {
            query = query.Where(x => x.OrganizationId == null || organizationIds.Contains(x.OrganizationId.Value));
        }

        return await query
            .Select(x => new SubOrganizationDto
            {
                Id = x.Id,
                OrganizationId = x.OrganizationId ?? 0,
                Name = x.Name!
            })
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<SiteDto>> GetSitesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Sites
            .Where(x => x.IsActive == true)
            .Select(x => new SiteDto { Id = x.Id, Name = x.SiteCode! })
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<SupervisorDto>> GetSupervisorsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Supervisors
            .Where(x => x.IsActive == true)
            .Select(x => new SupervisorDto { Id = x.Id, Name = x.Employee!.LastName + ", " + x.Employee!.FirstName })
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<ManagerDto>> GetManagersAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Managers
            .Where(x => x.IsActive == true)
            .Select(x => new ManagerDto { Id = x.Id, Name = x.Employee!.LastName + ", " + x.Employee!.FirstName })
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}
