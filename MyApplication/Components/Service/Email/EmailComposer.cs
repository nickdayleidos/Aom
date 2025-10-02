#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Data;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Services.Email;
using AomModel = MyApplication.Components.Model.AOM;

namespace MyApplication.Components.Services.Email
{
    public sealed class EmailComposer : IEmailComposer
    {
        private readonly AomDbContext _db;
        public EmailComposer(AomDbContext db) => _db = db;

        // Interval
        public async Task<(string Subject, string BodyHtml, string To, string Cc, string From)> ComposeAsync(
            string templateName, IntervalEmailContext ctx, CancellationToken ct = default)
        {
            var tpl = await GetTemplateAsync(templateName, ct);
            var (subject, bodyHtml) = IntervalEmailBuilder.Build(tpl, ctx);
            var to = NormalizeRecipients(tpl.ToAddresses);
            var cc = NormalizeRecipients(tpl.CcAddresses);
            var from = (tpl.SendFromAddress ?? string.Empty).Trim();
            return (subject, bodyHtml, to, cc, from);
        }

        // Operational Impact
        public async Task<(string Subject, string BodyHtml, string To, string Cc, string From)> ComposeAsync(
            string templateName, OiEventContext ctx, CancellationToken ct = default)
        {
            var tpl = await GetTemplateAsync(templateName, ct);
            var (subject, bodyHtml) = OiEmailBuilder.Build(tpl, ctx);
            var to = NormalizeRecipients(tpl.ToAddresses);
            var cc = NormalizeRecipients(tpl.CcAddresses);
            var from = (tpl.SendFromAddress ?? string.Empty).Trim();
            return (subject, bodyHtml, to, cc, from);
        }

        // Proactive (FIXED: call the builder)
        public async Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
            ComposeAsync(string templateName, ProactiveEmailContext ctx, CancellationToken ct = default)
        {
            var tpl = await GetTemplateAsync(templateName, ct).ConfigureAwait(false);

            var (subject, bodyHtml) = ProactiveEmailBuilder.Build(
                tpl.Subject ?? string.Empty,
                tpl.Body,
                ctx);

            var to = NormalizeRecipients(tpl.ToAddresses);
            var cc = NormalizeRecipients(tpl.CcAddresses);
            var from = (tpl.SendFromAddress ?? string.Empty).Trim();

            return (subject, bodyHtml, to, cc, from);
        }

        // helpers (unchanged) ...
        private static string NormalizeRecipients(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

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

        private async Task<EmailTemplates> GetTemplateAsync(string templateName, CancellationToken ct) =>
            await _db.EmailTemplates.AsNoTracking()
                   .Where(t => t.TemplateName == templateName && t.IsActive)
                   .SingleOrDefaultAsync(ct)
            ?? throw new InvalidOperationException($"Email template '{templateName}' not found or inactive.");
    }
}
