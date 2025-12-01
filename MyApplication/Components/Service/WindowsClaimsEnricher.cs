using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Security;

namespace MyApplication.Components.Service
{
    public sealed class WindowsClaimsEnricher : IClaimsTransformation
    {
        private readonly IDbContextFactory<AomDbContext> _dbFactory;

        public WindowsClaimsEnricher(IDbContextFactory<AomDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // We need the underlying Windows Identity to perform IsInRole checks against AD Groups
            if (principal.Identity is not WindowsIdentity wid || !wid.IsAuthenticated)
                return principal;

            // Avoid running twice
            if (wid.HasClaim(c => c.Type == "Debug:EnricherRan"))
                return principal;

            wid.AddClaim(new Claim("Debug:EnricherRan", DateTime.Now.ToString()));

            // Wrap in WindowsPrincipal to unlock the native .IsInRole() capability
            var wp = new WindowsPrincipal(wid);

            using (var ctx = await _dbFactory.CreateDbContextAsync())
            {
                // Fetch ALL assignments (cached/scoped context makes this cheap)
                var allAssignments = await ctx.AppRoleAssignments
                    .AsNoTracking()
                    .Include(a => a.AppRole)
                    .ToListAsync();

                // ---------------------------------------------------------
                // 1. CHECK USERS (Direct String Match)
                // ---------------------------------------------------------
                var userMatches = allAssignments
                    .Where(a => a.Type == "User" &&
                                string.Equals(a.Identifier, wid.Name, StringComparison.OrdinalIgnoreCase));

                foreach (var match in userMatches)
                {
                    AddRole(wid, match.AppRole.Name, "UserMatch");
                }

                // ---------------------------------------------------------
                // 2. CHECK GROUPS (The "Previous Setup" Logic)
                // ---------------------------------------------------------
                // We iterate the DB Groups and ask Windows: "Is this user in this group?"
                // This bypasses the need to translate SIDs manually.
                var groupAssignments = allAssignments.Where(a => a.Type == "Group");

                foreach (var assignment in groupAssignments)
                {
                    try
                    {
                        // assignment.Identifier should be "LEIDOS-CORP\Group"
                        if (wp.IsInRole(assignment.Identifier))
                        {
                            AddRole(wid, assignment.AppRole.Name, "GroupMatch");
                        }
                    }
                    catch
                    {
                        // Ignore invalid groups in DB (e.g. typos) that cause IsInRole to throw
                    }
                }
            }

            return principal;
        }

        private void AddRole(ClaimsIdentity id, string roleName, string source)
        {
            // Use the Identity's specific Role Claim Type (usually matches what policies look for)
            if (!id.HasClaim(id.RoleClaimType, roleName))
            {
                id.AddClaim(new Claim(id.RoleClaimType, roleName));
                id.AddClaim(new Claim($"Debug:{source}", roleName));
            }
        }
    }
}