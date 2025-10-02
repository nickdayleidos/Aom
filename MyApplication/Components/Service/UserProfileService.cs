#nullable enable
using System.Runtime.Versioning;
using System.Security.Claims;
using System.Security.Principal;

namespace MyApplication.Components.Service
{
    /// <summary>
    /// POCO with everything we know about the signed-in user.
    /// </summary>
    public sealed class UserProfile
    {
        public string? DisplayName { get; init; }
        public string? UserName { get; init; }          // DOMAIN\User or UPN
        public string? Email { get; init; }
        public string? Upn { get; init; }
        public string? Sid { get; init; }
        public string? IdentityProvider { get; init; }  // "Negotiate", "AzureAD", etc.
        public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> Groups { get; init; } = Array.Empty<string>(); // names on Windows, SIDs elsewhere
        public IReadOnlyDictionary<string, string> AllClaims { get; init; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Windows-safe helper: translates Group SIDs to names on Windows, returns SIDs elsewhere.
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Returns group names on Windows; on non-Windows returns the raw SIDs.
        /// </summary>
        public static IReadOnlyList<string> GetGroupNames(ClaimsPrincipal user)
        {
            if (!OperatingSystem.IsWindows())
            {
                // Cross-platform fallback: keep SIDs (still useful for authorization).
                return user.FindAll(ClaimTypes.GroupSid)
                           .Select(c => c.Value)
                           .Distinct()
                           .ToList();
            }

            return ResolveWindowsGroupNames(user);
        }

        // Windows-only API usage lives here; the attribute satisfies CA1416.
        [SupportedOSPlatform("windows")]
        private static List<string> ResolveWindowsGroupNames(ClaimsPrincipal user)
        {
            var names = new List<string>();

            foreach (var c in user.FindAll(ClaimTypes.GroupSid))
            {
                try
                {
                    var si = new SecurityIdentifier(c.Value);
                    var nt = (NTAccount)si.Translate(typeof(NTAccount));
                    names.Add(nt.ToString());
                }
                catch
                {
                    // Ignore translation failures; optionally log if desired.
                }
            }

            return names.Distinct().OrderBy(x => x).ToList();
        }
    }

    /// <summary>
    /// Builds a complete UserProfile from a ClaimsPrincipal.
    /// Inject and call GetAsync() from Blazor components/services.
    /// </summary>
    public sealed class UserProfileService
    {
        private readonly Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider _auth;

        public UserProfileService(Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider auth)
            => _auth = auth;

        public async Task<UserProfile> GetAsync()
        {
            var authState = await _auth.GetAuthenticationStateAsync();
            var user = authState.User;

            // Group all claims into a dictionary for easy display/use.
            var claimMap = user.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => string.Join(";", g.Select(x => x.Value)));

            string? FirstOf(params string[] keys)
            {
                foreach (var k in keys)
                {
                    if (claimMap.TryGetValue(k, out var v))
                        return v;
                }
                return null;
            }

            // Common claim types across Windows/Negotiate and Entra ID (Azure AD).
            var displayName = FirstOf(ClaimTypes.Name, "name");
            var userName    = FirstOf(ClaimTypes.WindowsAccountName, ClaimTypes.Name, "preferred_username", "upn");
            var email       = FirstOf(ClaimTypes.Email, "email");
            var upn         = FirstOf("upn", "preferred_username");
            var sid         = FirstOf(ClaimTypes.PrimarySid, ClaimTypes.NameIdentifier);

            // Roles (if emitted)
            var roles = user.FindAll(ClaimTypes.Role)
                            .Select(c => c.Value)
                            .Distinct()
                            .ToList();

            // Groups: names on Windows, SIDs elsewhere (CA1416-safe).
            var groups = ClaimsExtensions.GetGroupNames(user);

            return new UserProfile
            {
                DisplayName = displayName,
                UserName = userName,
                Email = email,
                Upn = upn,
                Sid = sid,
                IdentityProvider = user.Identity?.AuthenticationType,
                Roles = roles,
                Groups = groups,
                AllClaims = new Dictionary<string, string>(claimMap, StringComparer.OrdinalIgnoreCase)
            };
        }
    }
}
