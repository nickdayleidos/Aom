using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service
{
    public sealed class ProactiveRepository : IProactiveRepository
    {
        private readonly AomDbContext _db;
        public ProactiveRepository(AomDbContext db) => _db = db;

        public async Task<ProactiveAnnouncement?> GetLatestAsync(CancellationToken ct = default)
            => await _db.ProactiveAnnouncements.AsNoTracking().OrderByDescending(x => x.ProactiveTime).FirstOrDefaultAsync(ct);

        public async Task<ProactiveAnnouncement> InsertAsync(ProactiveAnnouncement entity, CancellationToken ct = default)
        {
            entity.ProactiveTime = GetEasternNow();  // stamp ET
            _db.ProactiveAnnouncements.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;   // FIX: return the entity, not an int
        }


        private static DateTime GetEasternNow()
        {
            try { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")); }
            catch { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/New_York")); }
        }
    }
}
