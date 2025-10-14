#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM;
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Services.Email
{
    public sealed class EmailComposer : IEmailComposer
    {
        private readonly AomDbContext _db;
        public EmailComposer(AomDbContext db) => _db = db;

        // -------- Interval --------
        public async Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
            ComposeAsync(string templateName, IntervalEmailContext ctx, CancellationToken ct = default)
        {
            var tpl = await GetTemplateAsync(templateName, ct).ConfigureAwait(false);
            var (subject, bodyHtml) = IntervalEmailBuilder.Build(tpl, ctx);

            return (subject,
                    bodyHtml,
                    NormalizeRecipients(tpl.ToAddresses),
                    NormalizeRecipients(tpl.CcAddresses),
                    (tpl.SendFromAddress ?? string.Empty).Trim());
        }

        // -------- Operational Impact --------
        public async Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
            ComposeAsync(string templateName, OiEventContext ctx, CancellationToken ct = default)
        {
            var tpl = await GetTemplateAsync(templateName, ct).ConfigureAwait(false);
            var (subject, bodyHtml) = OiEmailBuilder.Build(tpl, ctx);

            return (subject,
                    bodyHtml,
                    NormalizeRecipients(tpl.ToAddresses),
                    NormalizeRecipients(tpl.CcAddresses),
                    (tpl.SendFromAddress ?? string.Empty).Trim());
        }

        // -------- Proactive --------
        public async Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
            ComposeAsync(string templateName, ProactiveEmailContext ctx, CancellationToken ct = default)
        {
            var tpl = await GetTemplateAsync(templateName, ct).ConfigureAwait(false);

            var (subject, bodyHtml) = ProactiveEmailBuilder.Build(
                tpl.Subject ?? string.Empty,
                tpl.Body,
                ctx);

            return (subject,
                    bodyHtml,
                    NormalizeRecipients(tpl.ToAddresses),
                    NormalizeRecipients(tpl.CcAddresses),
                    (tpl.SendFromAddress ?? string.Empty).Trim());
        }

        // ---------- helpers ----------
        private static string NormalizeRecipients(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            // allow ',', ';', or newlines; de-dupe; trim entries
            var parts = s.Replace(',', ';')
                         .Replace('\n', ';')
                         .Replace('\r', ';')
                         .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in parts)
            {
                var t = p.Trim();
                if (t.Length > 0) set.Add(t);
            }

            return string.Join(';', set);
        }

        private async Task<EmailTemplates> GetTemplateAsync(string templateName, CancellationToken ct)
        {
            var tpl = await _db.EmailTemplates
                               .AsNoTracking()
                               .Where(t => t.TemplateName == templateName && t.IsActive)
                               .SingleOrDefaultAsync(ct)
                               .ConfigureAwait(false);

            return tpl ?? throw new InvalidOperationException(
                $"Email template '{templateName}' not found or inactive.");
        }
    }
}
