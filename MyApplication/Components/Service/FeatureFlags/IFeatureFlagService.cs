using MyApplication.Components.Common.Auth;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service.FeatureFlags
{
    public interface IFeatureFlagService
    {
        /// <summary>Returns true if the module is enabled (master switch on).</summary>
        Task<bool> IsEnabledAsync(string key);

        /// <summary>
        /// Returns true if the current user can access the feature.
        /// - If module is disabled: always false.
        /// - If AllowedRoles is set: true if any user role matches the CSV list.
        /// - If AllowedRoles is null: true (caller relies on its own [Authorize] check).
        /// </summary>
        Task<bool> CanAccessAsync(string key, UserRoles roles);

        Task<List<FeatureFlag>> GetAllAsync();
        Task SaveAsync(FeatureFlag flag);

        /// <summary>Clears the in-memory cache so changes take effect immediately.</summary>
        void InvalidateCache();
    }
}