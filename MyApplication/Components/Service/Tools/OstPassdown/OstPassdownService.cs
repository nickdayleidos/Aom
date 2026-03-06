using Microsoft.EntityFrameworkCore;
using MyApplication.Common.Time;
using MyApplication.Components.Common.Time;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service.Tools.OstPassdown
{
    public class OstPassdownService : IOstPassdownService
    {
        private readonly IDbContextFactory<AomDbContext> _factory;

        public OstPassdownService(IDbContextFactory<AomDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<Model.AOM.Tools.OstPassdown?> GetCurrentPassdownAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            // Returns the most recently posted passdown
            return await db.OstPassdown
                .Include(x => x.NewEdl)
                .Include(x => x.PrevEdl)
                .Include(x => x.ReskillBy)
                .Include(x => x.ProactiveBy)
                .Include(x => x.HomeportBy)
                .Include(x => x.SharepointBy)
                .OrderByDescending(x => x.PostedTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Model.AOM.Tools.OstPassdown>> GetHistoryAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.OstPassdown
                .Include(x => x.NewEdl)
                .Include(x => x.PrevEdl)
                .OrderByDescending(x => x.PostedTime)
                .ToListAsync();
        }

        public async Task<Model.AOM.Tools.OstPassdown?> GetPassdownByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.OstPassdown
                .Include(x => x.NewEdl)
                .Include(x => x.PrevEdl)
                .Include(x => x.ReskillBy)
                .Include(x => x.ProactiveBy)
                .Include(x => x.HomeportBy)
                .Include(x => x.SharepointBy)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreatePassdownAsync(Model.AOM.Tools.OstPassdown passdown, string user)
        {
            await using var db = await _factory.CreateDbContextAsync();
            passdown.PostedBy = user;
            passdown.PostedTime = Et.Now;
            db.OstPassdown.Add(passdown);
            await db.SaveChangesAsync();
        }

        public async Task UpdatePassdownAsync(Model.AOM.Tools.OstPassdown passdown, string user)
        {
            await using var db = await _factory.CreateDbContextAsync();
            passdown.UpdatedBy = user;
            passdown.UpdateTime = Et.Now;
            db.OstPassdown.Update(passdown);
            await db.SaveChangesAsync();
        }

        public async Task<List<Employees>> GetOstEmployeesAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            var targetOrgId = 5;
            var targetSubOrgId = 20;

            // Revised query to avoid EF Core translation issues with GroupBy/FirstOrDefault (EmptyProjectionMember error)
            // We select employees where their *latest* history record matches the criteria.
            return await db.Employees
                .Select(e => new
                {
                    Employee = e,
                    LatestHistory = db.EmployeeHistory
                        .Where(h => h.EmployeeId == e.Id)
                        .OrderByDescending(h => h.EffectiveDate)
                        .FirstOrDefault()
                })
                .Where(x => x.LatestHistory != null
                && x.LatestHistory.OrganizationId == targetOrgId
                && x.LatestHistory.SubOrganizationId == targetSubOrgId && x.LatestHistory.IsActive == true)
                .Select(x => x.Employee)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }
    }
}