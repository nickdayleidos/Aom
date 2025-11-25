using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Common.Time; // Added

namespace MyApplication.Components.Service
{
    public sealed class ProactiveRepository : IProactiveRepository
    {
        private readonly IDbContextFactory<AomDbContext> _factory;
        
        public ProactiveRepository(IDbContextFactory<AomDbContext> factory) => _factory = factory;

        public async Task<ProactiveAnnouncement?> GetLatestAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.ProactiveAnnouncements
                  .AsNoTracking()
                  .OrderByDescending(x => x.ProactiveTime)
                  .FirstOrDefaultAsync(ct);
        }

        public async Task<ProactiveAnnouncement> InsertAsync(ProactiveAnnouncement entity, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            
            // EFFICIENCY: Use Et.Now
            entity.ProactiveTime = Et.Now;
            
            db.ProactiveAnnouncements.Add(entity);
            await db.SaveChangesAsync(ct);
            
            return entity; 
        }
    }
}