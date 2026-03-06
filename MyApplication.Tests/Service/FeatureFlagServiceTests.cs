using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyApplication.Components.Common.Auth;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Service.FeatureFlags;
using NSubstitute;

namespace MyApplication.Tests.Service;

public class FeatureFlagServiceTests
{
    private static IMemoryCache MakeCache() =>
        new MemoryCache(Options.Create(new MemoryCacheOptions()));

    private static IFeatureFlagService MakeServiceWithFlags(params FeatureFlag[] flags)
    {
        var cache = MakeCache();
        // Pre-populate cache so no DB is needed
        var dict = flags.ToDictionary(f => f.Key, f => f, StringComparer.OrdinalIgnoreCase);
        cache.Set("aom:feature_flags", dict);

        var dbFactory = Substitute.For<Microsoft.EntityFrameworkCore.IDbContextFactory<MyApplication.Components.Data.AomDbContext>>();
        return new FeatureFlagService(dbFactory, cache);
    }

    [Fact]
    public async Task IsEnabledAsync_ReturnsFalse_WhenFlagDisabled()
    {
        var svc = MakeServiceWithFlags(new FeatureFlag { Key = "Module.ACR", DisplayName = "ACR", IsEnabled = false });
        (await svc.IsEnabledAsync("Module.ACR")).Should().BeFalse();
    }

    [Fact]
    public async Task IsEnabledAsync_ReturnsTrue_WhenFlagEnabled()
    {
        var svc = MakeServiceWithFlags(new FeatureFlag { Key = "Module.ACR", DisplayName = "ACR", IsEnabled = true });
        (await svc.IsEnabledAsync("Module.ACR")).Should().BeTrue();
    }

    [Fact]
    public async Task IsEnabledAsync_ReturnsTrue_WhenKeyUnknown()
    {
        var svc = MakeServiceWithFlags(); // empty
        (await svc.IsEnabledAsync("Unknown.Key")).Should().BeTrue(); // fail open
    }

    [Fact]
    public async Task CanAccessAsync_ReturnsFalse_WhenModuleDisabled()
    {
        var svc = MakeServiceWithFlags(new FeatureFlag { Key = "Module.WFM", DisplayName = "WFM", IsEnabled = false });
        var roles = new UserRoles { IsAdmin = true }; // admin, but still blocked
        (await svc.CanAccessAsync("Module.WFM", roles)).Should().BeFalse();
    }

    [Fact]
    public async Task CanAccessAsync_ReturnsTrue_WhenRoleMatches_AllowedRoles()
    {
        var svc = MakeServiceWithFlags(new FeatureFlag
        {
            Key = "Module.WFM", DisplayName = "WFM", IsEnabled = true, AllowedRoles = "Admin,WFM"
        });
        var roles = new UserRoles { IsWfm = true };
        (await svc.CanAccessAsync("Module.WFM", roles)).Should().BeTrue();
    }

    [Fact]
    public async Task CanAccessAsync_ReturnsFalse_WhenRoleNotInAllowedRoles()
    {
        var svc = MakeServiceWithFlags(new FeatureFlag
        {
            Key = "Module.WFM", DisplayName = "WFM", IsEnabled = true, AllowedRoles = "Admin"
        });
        var roles = new UserRoles { IsTraining = true }; // not in AllowedRoles
        (await svc.CanAccessAsync("Module.WFM", roles)).Should().BeFalse();
    }

    [Fact]
    public async Task CanAccessAsync_ReturnsTrue_WhenNoRoleOverride()
    {
        var svc = MakeServiceWithFlags(new FeatureFlag
        {
            Key = "Module.ACR", DisplayName = "ACR", IsEnabled = true, AllowedRoles = null
        });
        var roles = new UserRoles(); // no roles
        // No override = caller handles auth themselves
        (await svc.CanAccessAsync("Module.ACR", roles)).Should().BeTrue();
    }

    [Fact]
    public void InvalidateCache_RemovesCachedEntry()
    {
        var cache = MakeCache();
        cache.Set("aom:feature_flags", new Dictionary<string, FeatureFlag>());
        cache.TryGetValue("aom:feature_flags", out _).Should().BeTrue();

        var dbFactory = Substitute.For<Microsoft.EntityFrameworkCore.IDbContextFactory<MyApplication.Components.Data.AomDbContext>>();
        var svc = new FeatureFlagService(dbFactory, cache);
        svc.InvalidateCache();

        cache.TryGetValue("aom:feature_flags", out _).Should().BeFalse();
    }
}
