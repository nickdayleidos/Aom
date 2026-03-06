using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using MyApplication.Components.Data;
using AomAppRoleAssignment = MyApplication.Components.Model.AOM.Security.AppRoleAssignment;

namespace MyApplication.Components.Service
{
    /// <summary>
    /// Middleware that enriches user claims AFTER authentication is complete.
    /// This runs after the auth pipeline, so Graph API tokens are available.
    /// AppRoleAssignments are cached in IMemoryCache (5-min TTL) to avoid a DB
    /// round-trip on every new login.
    /// </summary>
    public class ClaimsEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;
        private const string RoleAssignmentsCacheKey = "aom:role_assignments";
        private static readonly TimeSpan RoleAssignmentsCacheTtl = TimeSpan.FromMinutes(5);

        public ClaimsEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IDbContextFactory<AomDbContext> dbFactory,
            GraphServiceClient graphClient,
            IMemoryCache cache)
        {
            // Only enrich if user is authenticated and hasn't been enriched yet
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var identity = context.User.Identity as ClaimsIdentity;

                if (identity != null && !identity.HasClaim(c => c.Type == "ClaimsEnriched"))
                {
                    await EnrichClaimsAsync(identity, dbFactory, graphClient, cache);
                }
            }

            await _next(context);
        }

        private async Task EnrichClaimsAsync(
            ClaimsIdentity identity,
            IDbContextFactory<AomDbContext> dbFactory,
            GraphServiceClient graphClient,
            IMemoryCache cache)
        {
            // Mark as enriched to avoid running multiple times per session
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

            // Load role assignments from cache; fall back to DB on miss
            List<AomAppRoleAssignment> allAssignments;
            if (!cache.TryGetValue(RoleAssignmentsCacheKey, out allAssignments!))
            {
                try
                {
                    using var ctx = await dbFactory.CreateDbContextAsync();
                    allAssignments = await ctx.AppRoleAssignments
                        .AsNoTracking()
                        .Include(a => a.AppRole)
                        .ToListAsync();

                    cache.Set(RoleAssignmentsCacheKey, allAssignments,
                        new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = RoleAssignmentsCacheTtl });
                }
                catch (Exception ex)
                {
                    identity.AddClaim(new Claim("Debug:DatabaseError", ex.Message));
                    allAssignments = new List<AomAppRoleAssignment>();
                }
            }

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
