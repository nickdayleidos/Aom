// MyApplication/Components/Services/Email/OiEmailBuilder.cs
using System;
using System.Net;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Services.Email
{
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

            // Subject/body can be null in DB; treat as empty
            var subject = ReplaceTokens(tpl.Subject ?? string.Empty, ctx, html: false);
            var body = ReplaceTokens(tpl.Body ?? string.Empty, ctx, html: true);


            return (subject, body);
        }

        private static string ReplaceTokens(string text, OiEventContext ctx, bool html)
        {
            // Encode only when writing into HTML body
            string E(string? s) => html ? WebUtility.HtmlEncode(s ?? string.Empty) : (s ?? string.Empty);

            string F(DateTime dt) => html ? WebUtility.HtmlEncode(dt.ToString("g")) : dt.ToString("g");

            return text
                .Replace("{{EventId}}", ctx.EventId.ToString())
                .Replace("{{Summary}}", E(ctx.Summary))
                .Replace("{{ServicesAffected}}", E(ctx.ServicesAffected))
                .Replace("{{TicketNumber}}", E(ctx.TicketNumber))
                .Replace("{{CategoryId}}", ctx.CategoryId.ToString())
                .Replace("{{SeverityId}}", ctx.SeverityId.ToString())
                .Replace("{{SiteId}}", ctx.SiteId.ToString())
                .Replace("{{UsersAffected}}", (ctx.UsersAffected?.ToString()) ?? string.Empty)
                .Replace("{{EstimatedTimeToResolve}}", E(ctx.EstimatedTimeToResolve))
                .Replace("{{PostedTime}}", F(ctx.PostedTime))
                .Replace("{{StartTime}}", F(ctx.StartTime))
                .Replace("{{Description}}", E(ctx.Description));
        }
    }
}
