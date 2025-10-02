using System.Net;
using System.Text;
using MyApplication.Common.Time;   // <-- ET helpers

namespace MyApplication.Components.Services.Email
{
    public static class ProactiveEmailBuilder
    {
        public static (string Subject, string BodyHtml) Build(
            string templateSubject,
            string? templateBody,
            ProactiveEmailContext ctx)
        {
            // ctx.ProactiveTimeEastern is already ET; don't convert again.
            var et = DateTime.SpecifyKind(ctx.ProactiveTime, DateTimeKind.Unspecified);

            var subjectBase = string.IsNullOrWhiteSpace(templateSubject)
                ? "Proactive Announcement"
                : templateSubject;

            var subject = $"{subjectBase} - {et.ToString(Et.SubjectFormat)} ET";

            var sb = new StringBuilder();
            sb.Append("""
                <html>
                <head>
                  <meta charset="utf-8" />
                  <style>
                    body { font-family: Segoe UI, Roboto, Arial, sans-serif; }
                    .meta { font-size:12px; color:#555; margin-bottom:18px; }
                    .card { border:1px solid #e3e3e3; border-radius:6px; padding:12px 14px; margin-bottom:14px; }
                    h3 { margin:0 0 8px 0; font-size:16px; }
                    pre { white-space:pre-wrap; font-family:inherit; margin:0; }
                  </style>
                </head>
                <body>
            """);

            // Single meta line in ET (idempotent formatting)
            sb.Append($@"<div class=""meta"">Proactive Time (ET): {et.ToString(Et.BodyFormat)} ET</div>");

            sb.Append("<div class=\"card\"><h3>USN Injection Announcement</h3><pre>");
            sb.Append(WebUtility.HtmlEncode(ctx.UsnInjectionAnnouncement));
            sb.Append("</pre></div>");

            sb.Append("<div class=\"card\"><h3>USN Site Announcement</h3><pre>");
            sb.Append(WebUtility.HtmlEncode(ctx.UsnSiteAnnouncement));
            sb.Append("</pre></div>");

            sb.Append("<div class=\"card\"><h3>USN Status Announcement</h3><pre>");
            sb.Append(WebUtility.HtmlEncode(ctx.UsnStatusAnnouncement));
            sb.Append("</pre></div>");

            sb.Append("</body></html>");
            return (subject, sb.ToString());
        }
    }
}
