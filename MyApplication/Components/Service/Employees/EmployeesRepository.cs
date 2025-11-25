using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Service.Employee.Dtos;

namespace MyApplication.Components.Service.Employee
{
    public sealed class EmployeesRepository
    {
        private readonly IDbContextFactory<AomDbContext> _factory;
        public EmployeesRepository(IDbContextFactory<AomDbContext> factory) => _factory = factory;

        public async Task<List<EmployeeListItem>> SearchAsync(string? query, bool activeOnly, int take = 200, CancellationToken ct = default)
        {
            query ??= string.Empty;
            var q = query.Trim();

            await using var db = await _factory.CreateDbContextAsync(ct);

            var baseQuery =
                from e in db.Employees
                let hist =
                    (from h in db.EmployeeHistory
                     where h.EmployeeId == e.Id
                     orderby h.EffectiveDate descending
                     select new
                     {
                         h.SupervisorId,
                         h.ManagerId,
                         h.OrganizationId,
                         h.SubOrganizationId,

                         ManagerName = (
                             from m in db.Managers
                             join me in db.Employees on m.EmployeeId equals me.Id
                             where m.Id == h.ManagerId
                             select me.LastName + ", " + me.FirstName
                         ).FirstOrDefault(),

                         SupervisorName = (
                             from s in db.Supervisors
                             join se in db.Employees on s.EmployeeId equals se.Id
                             where s.Id == h.SupervisorId
                             select se.LastName + ", " + se.FirstName
                         ).FirstOrDefault(),

                         OrgName = db.Organizations.Where(o => o.Id == h.OrganizationId).Select(o => o.Name).FirstOrDefault(),
                         SubOrgName = db.SubOrganizations.Where(so => so.Id == h.SubOrganizationId).Select(so => so.Name).FirstOrDefault(),
                     }).FirstOrDefault()
                select new { Emp = e, History = hist };

            if (activeOnly)
                baseQuery = baseQuery.Where(x => x.Emp.IsActive);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var isId = int.TryParse(q, out var idValue);
                baseQuery = baseQuery.Where(x =>
                       (x.Emp.FirstName != null && EF.Functions.Like(x.Emp.FirstName, $"%{q}%"))
                    || (x.Emp.LastName != null && EF.Functions.Like(x.Emp.LastName, $"%{q}%"))
                    || (isId && x.Emp.Id == idValue)
                    || (x.History.ManagerName != null && EF.Functions.Like(x.History.ManagerName, $"%{q}%"))
                    || (x.History.SupervisorName != null && EF.Functions.Like(x.History.SupervisorName, $"%{q}%"))
                    || (x.History.OrgName != null && EF.Functions.Like(x.History.OrgName, $"%{q}%"))
                    || (x.History.SubOrgName != null && EF.Functions.Like(x.History.SubOrgName, $"%{q}%"))
                );
            }

            return await baseQuery
                .OrderBy(x => x.Emp.LastName).ThenBy(x => x.Emp.FirstName)
                .Take(take)
                .Select(x => new EmployeeListItem
                {
                    Id = x.Emp.Id,
                    FirstName = x.Emp.FirstName,
                    LastName = x.Emp.LastName,
                    IsActive = x.Emp.IsActive,
                    Manager = x.History.ManagerName,
                    Supervisor = x.History.SupervisorName,
                    Organization = x.History.OrgName,
                    SubOrganization = x.History.SubOrgName,
                })
                .ToListAsync(ct);
        }

        public async Task<EmployeeDetailDto?> GetDetailAsync(int employeeId, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var emp = await db.Employees.FirstOrDefaultAsync(e => e.Id == employeeId, ct);
            if (emp == null) return null;

            var hist =
                await (from h in db.EmployeeHistory
                       where h.EmployeeId == employeeId
                       orderby h.EffectiveDate descending
                       select new
                       {
                           h.EffectiveDate,
                           h.ManagerId,
                           h.SupervisorId,
                           h.OrganizationId,
                           h.SubOrganizationId,
                           h.EmployerId,
                           h.SiteId,

                           ManagerName = (
                               from m in db.Managers
                               join me in db.Employees on m.EmployeeId equals me.Id
                               where m.Id == h.ManagerId
                               select me.LastName + ", " + me.FirstName
                           ).FirstOrDefault(),

                           SupervisorName = (
                               from s in db.Supervisors
                               join se in db.Employees on s.EmployeeId equals se.Id
                               where s.Id == h.SupervisorId
                               select se.LastName + ", " + se.FirstName
                           ).FirstOrDefault(),

                           OrgName = db.Organizations.Where(o => o.Id == h.OrganizationId).Select(o => o.Name).FirstOrDefault(),
                           SubOrgName = db.SubOrganizations.Where(so => so.Id == h.SubOrganizationId).Select(so => so.Name).FirstOrDefault(),
                           EmployerName = db.Employers.Where(e => e.Id == h.EmployerId).Select(e => e.Name).FirstOrDefault(),
                           SiteName = db.Sites.Where(s => s.Id == h.SiteId).Select(s => s.SiteCode).FirstOrDefault(),
                       }).FirstOrDefaultAsync(ct);

            

            

            return new EmployeeDetailDto
            {
                Id = emp.Id,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                CorporateId = emp.CorporateId,
                DomainLoginName = emp.DomainLoginName,
                IsActive = emp.IsActive,
                Manager = hist?.ManagerName,
                Supervisor = hist?.SupervisorName,
                Organization = hist?.OrgName,
                SubOrganization = hist?.SubOrgName,
                Employer = hist?.EmployerName,
                Site = hist?.SiteName,
                EffectiveDate = hist?.EffectiveDate,

                
            };
        }
    }
}
