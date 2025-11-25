using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service
{
    public sealed class ProactiveRepository : IProactiveRepository
    {
        private readonly AomDbContext _db;
        public ProactiveRepository(AomDbContext db) => _db = db;

        public Task<ProactiveAnnouncement?> GetLatestAsync(CancellationToken ct = default)
            => _db.ProactiveAnnouncements
                  .AsNoTracking()
                  .OrderByDescending(x => x.ProactiveTime)
                  .FirstOrDefaultAsync(ct);

        public async Task<ProactiveAnnouncement> InsertAsync(ProactiveAnnouncement entity, CancellationToken ct = default)
        {
            // Stamp ET here (or let DB default do it)
            entity.ProactiveTime = GetEasternNow();
            _db.ProactiveAnnouncements.Add(entity);
            await _db.SaveChangesAsync(ct).ConfigureAwait(false);
            return entity; // return the materialized entity with Id/ProactiveTime populated
        }

        private static DateTime GetEasternNow()
        {
            try
            {
                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            }
            catch
            {
                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
            }
        }
    }
}
