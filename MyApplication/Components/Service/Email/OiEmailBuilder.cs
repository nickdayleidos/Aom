using System.Net;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Service.Email;

/// <summary>
/// Expands tokens in AOM.EmailTemplates using an OiEventContext.
/// Supports {{Summary}}, {{ServicesAffected}}, {{TicketNumber}},
/// {{CategoryId}}, {{SeverityId}}, {{SiteId}}, {{UsersAffected}},
/// {{EstimatedTimeToResolve}}, {{PostedTime}}, {{StartTime}}, {{Description}}, {{EventId}}.
/// </summary>
public static class OiEmailBuilder
{
    public static (string Subject, string BodyHtml) Build(EmailTemplates tpl, OiEventContext ctx)
    {
        if (tpl is null) throw new ArgumentNullException(nameof(tpl));
        if (ctx is null) throw new ArgumentNullException(nameof(ctx));

        return (
            TemplateRenderer.Render(tpl.Subject, BuildTokens(ctx, html: false)),
            TemplateRenderer.Render(tpl.Body,    BuildTokens(ctx, html: true))
        );
    }

    private static Dictionary<string, object?> BuildTokens(OiEventContext ctx, bool html)
    {
        string E(string? s) => html ? WebUtility.HtmlEncode(s ?? string.Empty) : (s ?? string.Empty);
        string D(DateTime dt) => html ? WebUtility.HtmlEncode(dt.ToString("g")) : dt.ToString("g");

        return new Dictionary<string, object?>
        {
            ["EventId"]                = ctx.EventId,
            ["Summary"]                = E(ctx.Summary),
            ["ServicesAffected"]       = E(ctx.ServicesAffected),
            ["TicketNumber"]           = E(ctx.TicketNumber),
            ["CategoryId"]             = ctx.CategoryId,
            ["SeverityId"]             = ctx.SeverityId,
            ["SiteId"]                 = ctx.SiteId,
            ["UsersAffected"]          = ctx.UsersAffected?.ToString() ?? string.Empty,
            ["EstimatedTimeToResolve"] = E(ctx.EstimatedTimeToResolve),
            ["PostedTime"]             = D(ctx.PostedTime),
            ["StartTime"]              = D(ctx.StartTime),
            ["Description"]            = E(ctx.Description),
        };
    }
}
