#nullable enable
using System;
using System.Net;
using MyApplication.Common.Time;   // <-- use Et.Now

namespace MyApplication.Components.Services.Email
{
    public enum BannerType { Fouo, Cui }

    public static class EmailDraftExtensions
    {
        public static EmailDraft WithBanner(this EmailDraft draft, BannerType type, string title, string preparedBy, DateTime generatedEt)
        {
            draft.IncludeCui = (type == BannerType.Cui);

            var stripeText = (type == BannerType.Cui)
                ? "CONTROLLED UNCLASSIFIED INFORMATION (CUI)"
                : "FOR OFFICIAL USE ONLY (FOUO)";

            draft.BannerHtml =
                "<div style=\"padding:10px;margin:0 0 10px 0;" +
                "background:#0B5ED7;color:#fff;border-radius:6px;" +
                "font-weight:700;font-family:Segoe UI, Roboto, Arial, sans-serif;letter-spacing:.6px;text-transform:uppercase;\">" +
                stripeText + "</div>" +

                "<div style=\"padding:10px;margin:0 0 10px 0;" +
                "background:#e8f4ff;border:1px solid #bcd7f0;border-radius:8px;" +
                "font-family:Segoe UI, Roboto, Arial, sans-serif;color:#0b3a5b;\">" +
                $"<div style=\"font-weight:700;\">{WebUtility.HtmlEncode(title)}</div>" +
                $"<div><strong>Prepared by:</strong> {WebUtility.HtmlEncode(preparedBy)}</div>" +
                $"<div><strong>Generated (ET):</strong> {generatedEt:MM/dd/yyyy h:mm tt} ET</div>" +
                "</div>";

            return draft;
        }

        public static EmailDraft WithFouoBanner(this EmailDraft draft, string title, string preparedBy, DateTime generatedEt)
            => draft.WithBanner(BannerType.Fouo, title, preparedBy, generatedEt);

        public static EmailDraft WithCuiBanner(this EmailDraft draft, string title, string preparedBy, DateTime generatedEt)
            => draft.WithBanner(BannerType.Cui, title, preparedBy, generatedEt);

        // Legacy helpers – now use Et.Now so they’re centralized
        [Obsolete("Use overload with generatedEt (ET).")]
        public static EmailDraft WithFouoBanner(this EmailDraft draft, string title, string preparedBy)
            => draft.WithFouoBanner(title, preparedBy, Et.Now);

        [Obsolete("Use overload with generatedEt (ET).")]
        public static EmailDraft WithCuiBanner(this EmailDraft draft, string title, string preparedBy)
            => draft.WithCuiBanner(title, preparedBy, Et.Now);
    }
}
