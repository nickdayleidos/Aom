using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyApplication.Components.Common.Auth;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service.FeatureFlags
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly IDbContextFactory<AomDbContext> _dbFactory;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "aom:feature_flags";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public FeatureFlagService(IDbContextFactory<AomDbContext> dbFactory, IMemoryCache cache)
        {
            _dbFactory = dbFactory;
            _cache = cache;
        }

        public async Task<bool> IsEnabledAsync(string key)
        {
            var flag = await GetFlagAsync(key);
            return flag?.IsEnabled ?? true; // fail open: unknown keys are treated as enabled
        }

        public async Task<bool> CanAccessAsync(string key, UserRoles roles)
        {
            var flag = await GetFlagAsync(key);
            if (flag == null) return true; // unknown key = no restriction

            if (!flag.IsEnabled) return false;

            if (string.IsNullOrWhiteSpace(flag.AllowedRoles)) return true; // no override; caller uses [Authorize]

            var allowed = flag.AllowedRoles
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return allowed.Any(r => UserHasRole(roles, r));
        }

        public async Task<List<FeatureFlag>> GetAllAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.FeatureFlags.AsNoTracking().OrderBy(f => f.Key).ToListAsync();
        }

        public async Task SaveAsync(FeatureFlag flag)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            if (flag.Id == 0)
                db.FeatureFlags.Add(flag);
            else
                db.FeatureFlags.Update(flag);

            await db.SaveChangesAsync();
            InvalidateCache();
        }

        public void InvalidateCache() => _cache.Remove(CacheKey);

        private async Task<FeatureFlag?> GetFlagAsync(string key)
        {
            if (!_cache.TryGetValue(CacheKey, out Dictionary<string, FeatureFlag>? flags))
            {
                await using var db = await _dbFactory.CreateDbContextAsync();
                var list = await db.FeatureFlags.AsNoTracking().ToListAsync();
                flags = list.ToDictionary(f => f.Key, f => f, StringComparer.OrdinalIgnoreCase);
                _cache.Set(CacheKey, flags, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl });
            }
            return flags!.GetValueOrDefault(key);
        }

        private static bool UserHasRole(UserRoles roles, string roleName) =>
            roleName.Trim().ToLowerInvariant() switch
            {
                "admin"                 => roles.IsAdmin,
                "wfm"                   => roles.IsWfm,
                "ost"                   => roles.IsOst,
                "manager"               => roles.IsManager,
                "supervisor"            => roles.IsSupervisor,
                "techlead" or "tech lead" => roles.IsTechLead,
                "training"              => roles.IsTraining,
                _                       => false
            };
    }
}