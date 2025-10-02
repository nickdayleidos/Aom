// MyApplication/Components/Services/Email/TemplateRenderer.cs
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyApplication.Components.Services.Email;

public static class TemplateRenderer
{
    // Matches {{Token}} or {{Token:format}}
    private static readonly Regex _rx = new(@"\{\{\s*(?<key>[A-Za-z0-9_]+)(:(?<fmt>[^}]+))?\s*\}\}",
        RegexOptions.Compiled);

    public static string Render(string? template, IDictionary<string, object?> values, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(template)) return string.Empty;
        culture ??= CultureInfo.GetCultureInfo("en-US");

        return _rx.Replace(template, m =>
        {
            var key = m.Groups["key"].Value;
            var fmt = m.Groups["fmt"].Success ? m.Groups["fmt"].Value : null;

            if (!values.TryGetValue(key, out var val) || val is null) return string.Empty;

            if (fmt is not null && val is IFormattable f) return f.ToString(fmt, culture);
            return Convert.ToString(val, culture) ?? string.Empty;
        });
    }
}
