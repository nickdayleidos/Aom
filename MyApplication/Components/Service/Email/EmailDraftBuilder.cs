#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MimeKit;
using MsgKit;
using MyApplication.Common.Time;

// MsgKit aliases (avoid name collisions)
using MEmail = MsgKit.Email;
using MSender = MsgKit.Sender;
using MRepresenting = MsgKit.Representing;

namespace MyApplication.Components.Service.Email;

public static class EmailDraftBuilder
{
    // EML (unchanged)
    public static byte[] BuildEmlBytes(EmailDraft draft)
    {
        if (draft is null) throw new ArgumentNullException(nameof(draft));

        var html = ComposeHtml(draft);
        var msg = new MimeMessage();

        foreach (var t in SplitRecipients(draft.To))
            msg.To.Add(MailboxAddress.Parse(t));
        foreach (var c in SplitRecipients(draft.Cc))
            msg.Cc.Add(MailboxAddress.Parse(c));

        msg.Subject = draft.Subject ?? string.Empty;

        msg.Body = new BodyBuilder { HtmlBody = html }.ToMessageBody();

        if (draft.OpenAsDraft)
        {
            msg.Headers.Remove("X-Unsent");
            msg.Headers.Add("X-Unsent", "1");
            msg.Headers.Remove("Date");
            msg.Headers.Remove("Message-ID");
        }

        using var ms = new MemoryStream();
        msg.WriteTo(ms);
        return ms.ToArray();
    }

    // MSG (MsgKit only)
    public static byte[] BuildMsgBytes(EmailDraft draft)
    {
        if (draft is null) throw new ArgumentNullException(nameof(draft));

        var html = ComposeHtml(draft);

        var fromAddress = (draft.From ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(fromAddress))
            throw new ArgumentException("draft.From is required for .msg", nameof(draft));

        static string DisplayName(string email) =>
            email.Contains('@') ? email[..email.IndexOf('@')] : email;

        // >>> Force the authoring identity to the shared mailbox <<<
        using var email = new MEmail(
            sender: new MSender(fromAddress, DisplayName(fromAddress)),
            representing: new MRepresenting(fromAddress, DisplayName(fromAddress)),
            subject: draft.Subject ?? string.Empty
        )
        {
            BodyText = null,
            BodyHtml = html
        };

        // Make replies route to the shared mailbox and reinforce account selection
        email.ReplyToRecipients.AddTo(fromAddress, DisplayName(fromAddress));

        foreach (var t in SplitRecipients(draft.To))
            email.Recipients.AddTo(t, t);
        foreach (var c in SplitRecipients(draft.Cc))
            email.Recipients.AddCc(c, c);

        using var ms = new MemoryStream();
        email.Save(ms);
        return ms.ToArray();
    }

    // Shims (your pages already call these)
    public static byte[] SaveMsgDraft_SendAs(EmailDraft draft) => BuildMsgBytes(draft);
    public static void SaveMsgDraft_SendAs(EmailDraft draft, string outputMsgPath)
    {
        var bytes = BuildMsgBytes(draft);
        var dir = Path.GetDirectoryName(Path.GetFullPath(outputMsgPath))!;
        Directory.CreateDirectory(dir);
        File.WriteAllBytes(outputMsgPath, bytes);
    }

    // Helpers
    private static string ComposeHtml(EmailDraft draft)
    {
        var parts = new List<string>(2);
        if (!string.IsNullOrWhiteSpace(draft.BannerHtml))
            parts.Add(draft.BannerHtml!);
        parts.Add(draft.HtmlBody ?? string.Empty);
        return string.Concat(parts);
    }

    private static IEnumerable<string> SplitRecipients(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) yield break;
        foreach (var part in s.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = part.Trim();
            if (trimmed.Length > 0) yield return trimmed;
        }
    }
}

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

    // Legacy helpers — now use Et.Now so they're centralized
    [Obsolete("Use overload with generatedEt (ET).")]
    public static EmailDraft WithFouoBanner(this EmailDraft draft, string title, string preparedBy)
        => draft.WithFouoBanner(title, preparedBy, Et.Now);

    [Obsolete("Use overload with generatedEt (ET).")]
    public static EmailDraft WithCuiBanner(this EmailDraft draft, string title, string preparedBy)
        => draft.WithCuiBanner(title, preparedBy, Et.Now);
}
