#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Service;

namespace MyApplication.Components.Services.Email
{
    public sealed class OperationalImpactEmailService
    {
        private readonly IEmailComposer _composer;
        private readonly IOiEventRepository _repo;
        private readonly IOiLookupRepository _lookups;

        public OperationalImpactEmailService(
            IEmailComposer composer,
            IOiEventRepository repo,
            IOiLookupRepository lookups)
        {
            _composer = composer ?? throw new ArgumentNullException(nameof(composer));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _lookups = lookups ?? throw new ArgumentNullException(nameof(lookups));
        }

        // -------------------------
        // CREATE (new OI email)
        // -------------------------

        private static DateTime GetEasternNow()
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); // Windows
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);          // Kind = Unspecified (good)
            }
            catch
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");     // Linux
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);          // Kind = Unspecified (good)
            }
        }

        public async Task<(string subject, string html, string to, string cc, string from, int id)>
            CreateEventAndComposeAsync(OiEvent model, CancellationToken ct = default)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            // Save and get Id
            var id = await _repo.InsertAsync(model, ct).ConfigureAwait(false);

            var sev = await _lookups.GetSeverityAsync(model.SeverityId, ct).ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Severity not found.");

            var template = "OperationalImpact." + NormalizeKey(sev.Name);

            var ctx = new OiEventContext
            {
                EventId = id,
                Summary = model.Summary,
                ServicesAffected = model.ServicesAffected,
                TicketNumber = model.TicketNumber,
                CategoryId = model.CategoryId,
                SeverityId = model.SeverityId,
                SiteId = model.SiteId,
                UsersAffected = model.UsersAffected,
                EstimatedTimeToResolve = model.EstimatedTimeToResolve,
                PostedTime = model.PostedTime,
                StartTime = model.StartTime,
                ResolutionTime = model.ResolutionTime,
                Description = model.Description
            };

            var compose = await _composer.ComposeAsync(template, ctx, ct).ConfigureAwait(false);
            var tplSubject = compose.Subject;
            var to = compose.To ?? string.Empty;
            var cc = compose.Cc ?? string.Empty;
            var from = compose.From ?? string.Empty;

            var subject = $"{tplSubject} {sev.Name} - {NowEtString()}";
            var html = await BuildBodyAsync(model, sev, null, ct, history: null).ConfigureAwait(false);

            return (subject, html, to, cc, from, id);
        }

        // -------------------------
        // UPDATE (add update email)
        // -------------------------
        public async Task<(string subject, string html, string to, string cc, string from)>
            ComposeUpdateAsync(int eventId, string summaryUpdate, bool allClear = false, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(summaryUpdate))
                throw new ArgumentException("Update summary is required.", nameof(summaryUpdate));

            // Save the update (repo stamps UpdateTime in ET)
            await _repo.InsertUpdateAsync(
                new OiEventUpdate { EventId = eventId, Summary = summaryUpdate }, allClear, ct
            ).ConfigureAwait(false);

            // Load event
            var ev = await _repo.GetAsync(eventId, ct).ConfigureAwait(false)
                     ?? throw new InvalidOperationException("Event not found.");

            // If ALL CLEAR and no ResolutionTime yet, stamp it with ET now and persist
            if (allClear && !ev.ResolutionTime.HasValue)
            {
                ev.ResolutionTime = GetEasternNow(); // Unspecified ET
                await _repo.UpdateAsync(ev, ct).ConfigureAwait(false);
            }

            var sev = await _lookups.GetSeverityAsync(ev.SeverityId, ct).ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Severity not found.");

            var template = "OperationalImpactUpdate." + NormalizeKey(sev.Name);
            var ctx = new OiEventContext { EventId = eventId, Summary = summaryUpdate };

            var compose = await _composer.ComposeAsync(template, ctx, ct).ConfigureAwait(false);
            var tplSubject = compose.Subject;
            var to = compose.To ?? string.Empty;
            var cc = compose.Cc ?? string.Empty;
            var from = compose.From ?? string.Empty;

            var subject = $"{tplSubject} {sev.Name} - {NowEtString()}";
            if (allClear) subject = ReplaceUpdateWithAllClear(subject);

            var history = (await _repo.ListUpdatesAsync(eventId, ct).ConfigureAwait(false))
                          .OrderByDescending(h => h.UpdateTime)
                          .ToList();

            var html = await BuildBodyAsync(ev, sev, summaryUpdate, ct, history).ConfigureAwait(false);

            return (subject, html, to, cc, from);
        }

        // -------------------------
        // Body renderer (clean table + optional history)
        // -------------------------
        private async Task<string> BuildBodyAsync(
            OiEvent e,
            OiSeverity sev,
            string? summaryOverride,
            CancellationToken ct,
            IReadOnlyList<OiEventUpdate>? history)
        {
            var cats = await _lookups.GetCategoriesAsync(ct).ConfigureAwait(false);
            var sites = await _lookups.GetSitesAsync(ct).ConfigureAwait(false);

            var categoryName = cats.FirstOrDefault(c => c.Id == e.CategoryId)?.Name ?? e.CategoryId.ToString();
            var site = sites.FirstOrDefault(s => s.Id == e.SiteId);

            string siteDisplay = site?.SiteCode ?? e.SiteId.ToString();

            string H(string? s) => WebUtility.HtmlEncode(s ?? string.Empty);
            var summaryText = string.IsNullOrWhiteSpace(summaryOverride) ? e.Summary : summaryOverride;
            string usersAffected = e.UsersAffected.ToString();

            var body =
                "<div style=\"font-family:Segoe UI, Roboto, Arial, sans-serif;font-size:14px;color:#1f2937\">" +
                "  <div style=\"margin:0 0 10px 0\">Alcon,</div>" +
                "  <table style=\"border-collapse:collapse;width:100%;max-width:900px\">" +
                     Row("Summary", H(summaryText)) +
                     Row("Services Affected", H(e.ServicesAffected)) +
                     Row("Ticket Number", H(e.TicketNumber)) +
                     Row("Category", H(categoryName)) +
                     Row("Severity", H(sev.Name)) +
                     Row("# Affected", usersAffected) +
                     Row("ETR", H(e.EstimatedTimeToResolve)) +
                     Row("Posted Time", H(FormatEt(e.PostedTime))) +
                     Row("Start Time", H(FormatEt(e.StartTime))) +
                     Row("Resolution Time", H(FormatEtNullable(e.ResolutionTime))) +
                     Row("Description", H(e.Description)) +
                     Row("Sites Affected", H(siteDisplay)) +
                "  </table>";

            if (history is { Count: > 0 })
            {
                var items = string.Join("",
                    history.Select(h =>
                        "<li style=\"margin:0.25rem 0\">" +
                            $"<span style=\"color:#374151\">{H(FormatEt(h.UpdateTime))}</span> — {H(h.Summary)}" +
                        "</li>"));

                body +=
                    "<hr style=\"border:none;border-top:1px solid #e5e7eb;margin:12px 0\"/>" +
                    "<div style=\"font-weight:600;margin:6px 0\">Update History</div>" +
                    "<ul style=\"margin:0 0 0 1rem;padding:0\">" +
                      items +
                    "</ul>";
            }

            body += "</div>";
            return body;

            static string Row(string label, string value) =>
                "<tr>" +
                  "<td style=\"vertical-align:top;padding:6px 8px;font-weight:600;width:180px;color:#111827\">" + label + ":</td>" +
                  "<td style=\"vertical-align:top;padding:6px 8px;border-left:1px solid #f3f4f6\">" + value + "</td>" +
                "</tr>";
        }

        // -------------------------
        // Helpers
        // -------------------------
        private static string JoinEmails(string[]? arr) =>
            (arr is { Length: > 0 })
                ? string.Join(';', arr.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
                : string.Empty;

        private static string NormalizeKey(string? name)
        {
            var s = (name ?? string.Empty).Trim()
                                          .Replace(' ', '-')
                                          .Replace('/', '-')
                                          .Replace('\\', '-');
            return s;
        }

        private static string ReplaceUpdateWithAllClear(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject)) return subject ?? string.Empty;

            var replaced = Regex.Replace(subject, @"\bupdate\b", "ALL CLEAR",
                                         RegexOptions.IgnoreCase);
            if (string.Equals(replaced, subject, StringComparison.Ordinal))
                replaced = subject + " - ALL CLEAR";
            return replaced;
        }

        private static TimeZoneInfo GetEtTz()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); }
            catch { return TimeZoneInfo.FindSystemTimeZoneById("America/New_York"); }
        }

        // *** FIXED: Treat Unspecified as already ET — do not convert again ***
        private static string FormatEt(DateTime dt)
        {
            var et = GetEtTz();

            DateTime etTime = dt.Kind switch
            {
                DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(dt, et),
                DateTimeKind.Local => TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Local, et),
                DateTimeKind.Unspecified => dt, // stored/displayed as ET already
                _ => dt
            };

            return $"{etTime:MM/dd/yyyy h:mm tt} ET";
        }

        private static string FormatEtNullable(DateTime? dt) =>
            dt.HasValue ? FormatEt(dt.Value) : string.Empty;

        private static string NowEtString()
        {
            var et = GetEtTz();
            var nowEt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, et);
            return $"{nowEt:MM/dd/yyyy h:mm tt} ET";
        }
    }
}
