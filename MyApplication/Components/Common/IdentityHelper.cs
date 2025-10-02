using System.Security.Claims;

namespace MyApplication.Common;

public static class IdentityHelpers
{
    /// <summary>Returns "rosenblum" from "LEIDOS-CORP\rosenblum" or "rosenblum@corp" → "rosenblum".</summary>
    public static string GetSamAccount(ClaimsPrincipal user)
    {
        var name = user?.Identity?.Name ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;

        var slash = name.IndexOf('\\');
        if (slash >= 0 && slash < name.Length - 1) return name[(slash + 1)..];

        var at = name.IndexOf('@');
        if (at > 0) return name[..at];

        return name;
    }
}
