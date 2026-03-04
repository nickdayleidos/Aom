namespace MyApplication.Components.Common;

/// <summary>
/// Search helper for MudAutocomplete / MudSelect controls backed by
/// a KeyValuePair&lt;int, string&gt; collection.
/// </summary>
public static class KvSearchHelper
{
    /// <summary>
    /// Returns all items when <paramref name="value"/> is empty/null;
    /// otherwise filters items whose Value contains the search term
    /// (case-insensitive, ordinal).
    /// </summary>
    public static Task<IEnumerable<KeyValuePair<int, string>?>> Search(
        IEnumerable<KeyValuePair<int, string>> source,
        string? value)
    {
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(source.Select(x => (KeyValuePair<int, string>?)x));

        return Task.FromResult(
            source.Where(x => x.Value.Contains(value, StringComparison.OrdinalIgnoreCase))
                  .Select(x => (KeyValuePair<int, string>?)x));
    }
}
