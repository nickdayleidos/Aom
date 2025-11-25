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

            // Encode + convert newlines to <br> so Outlook preserves breaks
            static string EncodeWithBr(string? s)
            {
                if (string.IsNullOrEmpty(s)) return string.Empty;
                var enc = WebUtility.HtmlEncode(s);
                // Normalize newlines then convert to <br>
                enc = enc.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br>");
                return enc;
            }

            // Render an Outlook-safe “card” using a simple table + inline styles
            static string RenderCard(string heading, string contentHtml)
            {
                const string bodyStyle =
                    "font-family:Segoe UI,Roboto,Arial,sans-serif;" +
                    "font-size:14px;line-height:1.4;" +
                    "word-wrap:break-word;overflow-wrap:break-word;" +
                    "word-break:break-word;-ms-word-break:break-all;" + // Outlook/IE
                    "margin:0;";

                var sb = new StringBuilder();
                sb.Append("<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse:collapse;margin:0 0 14px 0;\">");
                sb.Append("<tr><td style=\"border:1px solid #e3e3e3;padding:12px 14px;mso-padding-alt:12px 14px;\">");
                sb.Append($"<h3 style=\"margin:0 0 8px 0;font-size:16px;font-family:Segoe UI,Roboto,Arial,sans-serif;font-weight:600;\">{heading}</h3>");
                sb.Append($"<div style=\"{bodyStyle}\">{contentHtml}</div>");
                sb.Append("</td></tr></table>");
                return sb.ToString();
            }

            var sb = new StringBuilder();
            sb.Append("""
                <html>
                  <head>
                    <meta charset="utf-8" />
                    <meta name="x-apple-disable-message-reformatting" />
                    <meta name="format-detection" content="telephone=no,address=no,email=no,date=no,url=no" />
                  </head>
                  <body style="margin:0;padding:0;word-wrap:break-word;overflow-wrap:break-word;">
            """);

            // Outer container table
            sb.Append("<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse:collapse;\">");
            sb.Append("<tr><td style=\"font-family:Segoe UI,Roboto,Arial,sans-serif;\">");

            // Meta line
            sb.Append($@"<div style=""font-size:12px;color:#555;margin:0 0 18px 0;font-family:Segoe UI,Roboto,Arial,sans-serif;"">
                           Proactive Time (ET): {et.ToString(Et.BodyFormat)} ET
                         </div>");

            void AppendSection(string heading, string? content)
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var html = EncodeWithBr(content);
                    sb.Append(RenderCard(heading, html));
                }
            }

            AppendSection("USN Injection Announcement", ctx.UsnInjectionAnnouncement);
            AppendSection("USN Site Announcement", ctx.UsnSiteAnnouncement);
            AppendSection("USN Status Announcement", ctx.UsnStatusAnnouncement);

            sb.Append("</td></tr></table>");
            sb.Append("</body></html>");

            return (subject, sb.ToString());
        }
    }
}
