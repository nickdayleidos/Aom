using System.Security.Claims;
using System.Security.Principal;                 // SecurityIdentifier, NTAccount
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Runtime.Versioning;

namespace MyApplication.Components.Service
{
    public sealed class WindowsClaimsEnricher : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is not ClaimsIdentity id || !id.IsAuthenticated)
                return Task.FromResult(principal);

            // Only enrich Negotiate/Windows identities
            var authType = id.AuthenticationType ?? string.Empty;
            if (!authType.Equals(NegotiateDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(principal);

            // Roles from groups (translate SIDs -> names on Windows; otherwise fall back to raw SIDs)
            foreach (var g in GetGroupNames(principal))
            {
                if (!id.HasClaim(ClaimTypes.Role, g))
                    id.AddClaim(new Claim(ClaimTypes.Role, g));
            }

#if WINDOWS
            // Email from AD (best-effort)
            if (!id.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                var email = LookupEmailFromAd(principal);
                if (!string.IsNullOrWhiteSpace(email))
                    id.AddClaim(new Claim(ClaimTypes.Email, email!));
            }
#endif

            return Task.FromResult(principal);
        }

        private static IEnumerable<string> GetGroupNames(ClaimsPrincipal principal)
        {
#if WINDOWS
            foreach (var sid in principal.Claims.Where(c => c.Type == ClaimTypes.GroupSid).Select(c => c.Value))
            {
                try
                {
                    var name = new SecurityIdentifier(sid).Translate(typeof(NTAccount)).ToString();
                    yield return name;
                }
                catch { /* ignore bad/foreign SIDs */ }
            }
#else
            // Non-Windows: you can only show SIDs (no translation available)
            foreach (var sid in principal.Claims.Where(c => c.Type == ClaimTypes.GroupSid).Select(c => c.Value))
                yield return sid;
#endif
        }

#if WINDOWS
        [SupportedOSPlatform("windows")]
        private static string? LookupEmailFromAd(ClaimsPrincipal user)
        {
            var sam = user.Identity?.Name?.Split('\\').LastOrDefault();
            if (string.IsNullOrWhiteSpace(sam)) return null;

            try
            {
                using var ctx = new System.DirectoryServices.AccountManagement.PrincipalContext(
                    System.DirectoryServices.AccountManagement.ContextType.Domain);
                using var up = System.DirectoryServices.AccountManagement.UserPrincipal.FindByIdentity(
                    ctx,
                    System.DirectoryServices.AccountManagement.IdentityType.SamAccountName,
                    sam);
                return up?.EmailAddress;
            }
            catch { return null; }
        }
#endif
    }
}
