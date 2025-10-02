// MyApplication/Components/Services/Email/IEmailComposer.cs
using System.Threading;

namespace MyApplication.Components.Services.Email
{
    public interface IEmailComposer
    {
        Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
            ComposeAsync(string templateName, IntervalEmailContext ctx, CancellationToken ct = default);

        Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
            ComposeAsync(string templateName, OiEventContext ctx, CancellationToken ct = default);
        Task<(string Subject, string BodyHtml, string To, string Cc, string From)>
            ComposeAsync(string templateName, ProactiveEmailContext ctx, CancellationToken ct = default);

    }
}
