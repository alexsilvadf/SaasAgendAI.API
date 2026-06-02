using AgendAI.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace AgendAI.Infra.Email;

public sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        logger.LogWarning(
            "E-mail não enviado (SMTP desabilitado). Para: {To} | Assunto: {Subject} | Corpo: {Body}",
            to,
            subject,
            htmlBody);

        return Task.CompletedTask;
    }
}
