using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using MyApplication.Components.Data;

namespace MyApplication.Components.Service
{
    /// <summary>
    /// Middleware that enriches user claims AFTER authentication is complete.
    /// This runs after the auth pipeline, so Graph API tokens are available.
    /// </summary>
    public class ClaimsEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;

        public ClaimsEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IDbContextFactory<AomDbContext> dbFactory,
            GraphServiceClient graphClient)
        {
            // Only enrich if user is authenticated and hasn't been enriched yet
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var identity = context.User.Identity as ClaimsIdentity;

                if (identity != null && !identity.HasClaim(c => c.Type == "ClaimsEnriched"))
                {
                    await EnrichClaimsAsync(identity, dbFactory, graphClient);
                }
            }

            await _next(context);
        }

        private async Task EnrichClaimsAsync(
            ClaimsIdentity identity,
            IDbContextFactory<AomDbContext> dbFactory,
            GraphServiceClient graphClient)
        {
            // Mark as enriched to avoid running multiple times
            identity.AddClaim(new Claim("ClaimsEnriched", "true"));

            // Try to get groups from Graph API
            List<string> userGraphGroups = new List<string>();
            try
            {
                var memberOfResult = await graphClient.Me.MemberOf.GetAsync();

                userGraphGroups = memberOfResult?.Value?
                    .OfType<Group>()
                    .Where(g => g.DisplayName != null)
                    .Select(g => g.DisplayName!)
                    .ToList() ?? new List<string>();

                identity.AddClaim(new Claim("Debug:GraphAPISuccess", $"Found {userGraphGroups.Count} groups"));
            }
            catch (Exception ex)
            {
                identity.AddClaim(new Claim("Debug:GraphAPIError", ex.Message));
            }

            // Always run database matching
            try
            {
                using (var ctx = await dbFactory.CreateDbContextAsync())
                {
                    var allAssignments = await ctx.AppRoleAssignments
                        .AsNoTracking()
                        .Include(a => a.AppRole)
                        .ToListAsync();

                    // 1. CHECK GROUPS
                    if (userGraphGroups.Any())
                    {
                        var groupAssignments = allAssignments.Where(a => a.Type == "Group");

                        foreach (var assignment in groupAssignments)
                        {
                            var groupNameToMatch = ExtractGroupName(assignment.Identifier);

                            if (userGraphGroups.Any(g => g.Equals(groupNameToMatch, StringComparison.OrdinalIgnoreCase)))
                            {
                                AddRole(identity, assignment.AppRole!.Name, "GroupMatch");
                            }
                        }
                    }

                    // 2. CHECK USERS
                    var userName = identity.FindFirst(ClaimTypes.Name)?.Value
                        ?? identity.FindFirst("preferred_username")?.Value
                        ?? identity.FindFirst(ClaimTypes.Email)?.Value;

                    if (!string.IsNullOrEmpty(userName))
                    {
                        var userMatches = allAssignments
                            .Where(a => a.Type == "User" &&
                                        (string.Equals(a.Identifier, userName, StringComparison.OrdinalIgnoreCase) ||
                                         string.Equals(a.Identifier, userName.Split('@')[0], StringComparison.OrdinalIgnoreCase)));

                        foreach (var match in userMatches)
                        {
                            AddRole(identity, match.AppRole!.Name, "UserMatch");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                identity.AddClaim(new Claim("Debug:DatabaseError", ex.Message));
            }
        }

        private string ExtractGroupName(string identifier)
        {
            if (identifier.Contains('\\'))
            {
                return identifier.Split('\\').Last();
            }
            return identifier;
        }

        private void AddRole(ClaimsIdentity identity, string roleName, string source)
        {
            if (!identity.HasClaim(identity.RoleClaimType, roleName))
            {
                identity.AddClaim(new Claim(identity.RoleClaimType, roleName));
                identity.AddClaim(new Claim($"Debug:{source}", roleName));
            }
        }
    }
}
