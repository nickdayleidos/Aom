// EmailDraft.cs
public sealed class EmailDraft
{
    public string Subject { get; set; } = "";
    public string HtmlBody { get; set; } = "";
    public string From { get; set; } = "";
    public string To { get; set; } = "";   // semicolon/comma delimited
    public string Cc { get; set; } = "";   // semicolon/comma delimited
    public bool OpenAsDraft { get; set; }

    // For banners
    public string? BannerHtml { get; set; }

    // NEW: flag for CUI inclusion
    public bool IncludeCui { get; set; }
}
