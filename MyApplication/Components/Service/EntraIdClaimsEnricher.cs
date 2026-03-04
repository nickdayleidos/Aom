using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using MyApplication.Components.Data;

namespace MyApplication.Components.Service
{
    public sealed class EntraIdClaimsEnricher : IClaimsTransformation
    {
        private readonly IDbContextFactory<AomDbContext> _dbFactory;
        private readonly GraphServiceClient _graphClient;
        private readonly ITokenAcquisition _tokenAcquisition;

        public EntraIdClaimsEnricher(
            IDbContextFactory<AomDbContext> dbFactory,
            GraphServiceClient graphClient,
            ITokenAcquisition tokenAcquisition)
        {
            _dbFactory = dbFactory;
            _graphClient = graphClient;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
                return principal;

            // Avoid running twice
            if (identity.HasClaim(c => c.Type == "Debug:EnricherRan"))
                return principal;

            identity.AddClaim(new Claim("Debug:EnricherRan", DateTime.Now.ToString()));

            // Try to get groups from Graph API (optional - may fail during claims transformation)
            List<string> userGraphGroups = new List<string>();
            try
            {
                // Get user's groups from Microsoft Graph API
                // Let GraphServiceClient handle token acquisition automatically
                var memberOfResult = await _graphClient.Me.MemberOf.GetAsync();

                userGraphGroups = memberOfResult?.Value?
                    .OfType<Group>()
                    .Where(g => g.DisplayName != null)
                    .Select(g => g.DisplayName!)
                    .ToList() ?? new List<string>();

                // Add debug claim to show Graph API succeeded
                identity.AddClaim(new Claim("Debug:GraphAPISuccess", $"Found {userGraphGroups.Count} groups"));
            }
            catch (Exception ex)
            {
                // Log the error but continue with user matching
                // This is expected during initial authentication before token is ready
                identity.AddClaim(new Claim("Debug:EnricherError", ex.Message));
            }

            // Always run database matching logic (even if Graph API failed)
            try
            {
                using (var ctx = await _dbFactory.CreateDbContextAsync())
                {
                    // Fetch ALL assignments
                    var allAssignments = await ctx.AppRoleAssignments
                        .AsNoTracking()
                        .Include(a => a.AppRole)
                        .ToListAsync();

                    // ---------------------------------------------------------
                    // 1. CHECK GROUPS (Graph API Groups)
                    // ---------------------------------------------------------
                    if (userGraphGroups.Any())
                    {
                        var groupAssignments = allAssignments.Where(a => a.Type == "Group");

                        foreach (var assignment in groupAssignments)
                        {
                            var groupNameToMatch = ExtractGroupName(assignment.Identifier);

                            // Check if user is in this group
                            if (userGraphGroups.Any(g => g.Equals(groupNameToMatch, StringComparison.OrdinalIgnoreCase)))
                            {
                                AddRole(identity, assignment.AppRole.Name, "GroupMatch");
                            }
                        }
                    }

                    // ---------------------------------------------------------
                    // 2. CHECK USERS (Direct Match) - ALWAYS RUN THIS
                    // ---------------------------------------------------------
                    var userName = principal.FindFirst(ClaimTypes.Name)?.Value
                        ?? principal.FindFirst("preferred_username")?.Value
                        ?? principal.FindFirst(ClaimTypes.Email)?.Value;

                    if (!string.IsNullOrEmpty(userName))
                    {
                        var userMatches = allAssignments
                            .Where(a => a.Type == "User" &&
                                        (string.Equals(a.Identifier, userName, StringComparison.OrdinalIgnoreCase) ||
                                         // Also try matching just the username part before @
                                         string.Equals(a.Identifier, userName.Split('@')[0], StringComparison.OrdinalIgnoreCase)));

                        foreach (var match in userMatches)
                        {
                            AddRole(identity, match.AppRole.Name, "UserMatch");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log database errors
                identity.AddClaim(new Claim("Debug:DatabaseError", ex.Message));
            }

            return principal;
        }

        private string ExtractGroupName(string identifier)
        {
            // If the identifier is in Windows format "DOMAIN\GroupName", extract just the group name
            // Otherwise, use the identifier as-is
            if (identifier.Contains('\\'))
            {
                return identifier.Split('\\').Last();
            }
            return identifier;
        }

        private void AddRole(ClaimsIdentity identity, string roleName, string source)
        {
            // Use the Identity's specific Role Claim Type
            if (!identity.HasClaim(identity.RoleClaimType, roleName))
            {
                identity.AddClaim(new Claim(identity.RoleClaimType, roleName));
                identity.AddClaim(new Claim($"Debug:{source}", roleName));
            }
        }
    }
}
