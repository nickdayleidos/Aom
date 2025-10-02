#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using MimeKit;
using MyApplication.Components.Services.Email;

public static class EmailDraftBuilder
{
    public static byte[] BuildEmlBytes(EmailDraft draft)
    {
        if (draft is null) throw new ArgumentNullException(nameof(draft));

        // Build HTML body with optional CUI banner and custom banner, then the main content
        var html = ComposeHtml(draft);

        var msg = new MimeMessage();

        // -----------------------------
        // Recipients
        // -----------------------------
        foreach (var t in SplitRecipients(draft.To))
            msg.To.Add(MailboxAddress.Parse(t));

        foreach (var c in SplitRecipients(draft.Cc))
            msg.Cc.Add(MailboxAddress.Parse(c));

        // -----------------------------
        // Subject
        // -----------------------------
        msg.Subject = draft.Subject ?? string.Empty;

        // -----------------------------
        // From / Sender (force pure Send-As draft)
        // -----------------------------
        msg.From.Clear();
        if (!string.IsNullOrWhiteSpace(draft.From))
            msg.From.Add(MailboxAddress.Parse(draft.From.Trim()));

        // Absolutely ensure no Sender/resent/transport headers that could trigger "on behalf of"
        msg.Sender = null;
        msg.Headers.Remove("Sender");
        msg.Headers.Remove("Return-Path");
        msg.Headers.Remove("Resent-From");
        msg.Headers.Remove("Resent-Sender");
        msg.Headers.Remove("Resent-To");
        msg.Headers.Remove("Resent-Date");

        // -----------------------------
        // Body (HTML only; add TextBody if you ever want a plain-text alt)
        // -----------------------------
        var bodyBuilder = new BodyBuilder { HtmlBody = html };
        msg.Body = bodyBuilder.ToMessageBody();

        // -----------------------------
        // Open as DRAFT in the mail client
        // -----------------------------
        if (draft.OpenAsDraft)
        {
            // Key flag for Outlook/Thunderbird compose window
            msg.Headers.Remove("X-Unsent");
            msg.Headers.Add("X-Unsent", "1");

            // Avoid looking like an already-sent message
            msg.Headers.Remove("Date");
            msg.Headers.Remove("Message-ID");
        }

        using var ms = new MemoryStream();
        msg.WriteTo(ms);
        return ms.ToArray();
    }

    private static string ComposeHtml(EmailDraft draft)
    {
        var parts = new List<string>(capacity: 2);

        // Only the blue banner you build in EmailDraftExtensions
        if (!string.IsNullOrWhiteSpace(draft.BannerHtml))
            parts.Add(draft.BannerHtml!);

        // Main body
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
