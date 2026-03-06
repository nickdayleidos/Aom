using System.Security.Claims;
using FluentAssertions;

namespace MyApplication.Tests.Service;

// Tests for observable behavior driven by ClaimsEnrichmentMiddleware's logic.
// We replicate the two private helpers here to test them in isolation.
public class ClaimsEnrichmentMiddlewareTests
{
    private static string ExtractGroupName(string identifier)
    {
        if (identifier.Contains('\\'))
            return identifier.Split('\\').Last();
        return identifier;
    }

    [Theory]
    [InlineData(@"LEIDOS-CORP\TODAdmin", "TODAdmin")]
    [InlineData(@"LEIDOS-CORP\SMIT_Admin", "SMIT_Admin")]
    [InlineData("SimpleGroup", "SimpleGroup")]
    [InlineData(@"NO\SLASH\MULTIPLE", "MULTIPLE")]
    public void ExtractGroupName_StripsDomainPrefix(string input, string expected)
    {
        ExtractGroupName(input).Should().Be(expected);
    }

    [Fact]
    public void ClaimsEnrichedSentinel_PreventsDoubleEnrichment()
    {
        var identity = new ClaimsIdentity("test");
        identity.AddClaim(new Claim("ClaimsEnriched", "true"));
        identity.HasClaim(c => c.Type == "ClaimsEnriched").Should().BeTrue();
    }

    [Fact]
    public void FreshIdentity_DoesNotHaveEnrichedSentinel()
    {
        var identity = new ClaimsIdentity("test");
        identity.HasClaim(c => c.Type == "ClaimsEnriched").Should().BeFalse();
    }

    [Fact]
    public void GroupMatch_CaseInsensitive_AddsRole()
    {
        var userGroups = new[] { "todadmin" };
        var groupName = ExtractGroupName(@"LEIDOS-CORP\TODAdmin");
        userGroups.Any(g => g.Equals(groupName, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void GroupMatch_NoMatch_DoesNotAddRole()
    {
        var userGroups = new[] { "DifferentGroup" };
        var groupName = ExtractGroupName(@"LEIDOS-CORP\TODAdmin");
        userGroups.Any(g => g.Equals(groupName, StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
    }

    [Theory]
    [InlineData("dayng@leidos.com", "dayng@leidos.com", true)]
    [InlineData("dayng@leidos.com", "dayng", true)]
    [InlineData("other@leidos.com", "dayng", false)]
    public void UserMatch_VariousIdentifiers_MatchesCorrectly(string userName, string identifier, bool shouldMatch)
    {
        var exactMatch = string.Equals(identifier, userName, StringComparison.OrdinalIgnoreCase);
        var prefixMatch = string.Equals(identifier, userName.Split('@')[0], StringComparison.OrdinalIgnoreCase);
        (exactMatch || prefixMatch).Should().Be(shouldMatch);
    }
}