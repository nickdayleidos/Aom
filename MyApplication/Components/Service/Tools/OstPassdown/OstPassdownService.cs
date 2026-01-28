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
        private readonly AomDbContext _context;

        public OstPassdownService(AomDbContext context)
        {
            _context = context;
        }

        public async Task<Model.AOM.Tools.OstPassdown?> GetCurrentPassdownAsync()
        {
            // Returns the most recently posted passdown
            return await _context.OstPassdown
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
            return await _context.OstPassdown
                .Include(x => x.NewEdl)
                .Include(x => x.PrevEdl)
                .OrderByDescending(x => x.PostedTime)
                .ToListAsync();
        }

        public async Task<Model.AOM.Tools.OstPassdown?> GetPassdownByIdAsync(int id)
        {
            return await _context.OstPassdown
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
            passdown.PostedBy = user;
            passdown.PostedTime = Et.Now;
            _context.OstPassdown.Add(passdown);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePassdownAsync(Model.AOM.Tools.OstPassdown passdown, string user)
        {
            passdown.UpdatedBy = user;
            passdown.UpdateTime = Et.Now;
            _context.OstPassdown.Update(passdown);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Employees>> GetOstEmployeesAsync()
        {
            var targetOrgId = 5;
            var targetSubOrgId = 20;

            // Revised query to avoid EF Core translation issues with GroupBy/FirstOrDefault (EmptyProjectionMember error)
            // We select employees where their *latest* history record matches the criteria.
            return await _context.Employees
                .Select(e => new
                {
                    Employee = e,
                    LatestHistory = _context.EmployeeHistory
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