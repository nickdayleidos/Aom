using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MyApplication.Components.Common.Auth;

public static class AuthorizationHelpers
{
    public static async Task<UserRoles> GetUserRolesAsync(
        this AuthenticationStateProvider provider)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        var user = authState.User;

        return new UserRoles
        {
            IsAdmin = user.IsInRole(RoleConstants.Admin),
            IsWfm = user.IsInRole(RoleConstants.WFM),
            IsOst = user.IsInRole(RoleConstants.OST),
            IsManager = user.IsInRole(RoleConstants.Manager),
            IsSupervisor = user.IsInRole(RoleConstants.Supervisor),
            IsTechLead = user.IsInRole(RoleConstants.TechLead),
            IsTraining = user.IsInRole(RoleConstants.Training)
        };
    }

    public static async Task<string?> GetUserNameAsync(
        this AuthenticationStateProvider provider)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        return authState.User.Identity?.Name;
    }

    public static async Task<IEnumerable<string>> GetUserRoleNamesAsync(
        this AuthenticationStateProvider provider)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        return authState.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }
}

public record UserRoles
{
    public bool IsAdmin { get; init; }
    public bool IsWfm { get; init; }
    public bool IsOst { get; init; }
    public bool IsManager { get; init; }
    public bool IsSupervisor { get; init; }
    public bool IsTechLead { get; init; }
    public bool IsTraining { get; init; }

    public bool IsManagement => IsAdmin || IsWfm || IsOst || IsManager || IsSupervisor;
}
