using AgendAI.Application.Abstractions;
using AgendAI.Application.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AgendAI.Infra.Email;

public sealed class SmtpEmailSender(
    IOptions<EmailOptions> emailOptions,
    ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var options = emailOptions.Value;
        if (string.IsNullOrWhiteSpace(options.SmtpHost))
            throw new InvalidOperationException("Email:SmtpHost não configurado.");

        if (string.IsNullOrWhiteSpace(options.FromAddress))
            throw new InvalidOperationException("Email:FromAddress não configurado.");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.FromName, options.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        var secureSocket = ResolverSeguranca(options);

        try
        {
            await client.ConnectAsync(options.SmtpHost, options.SmtpPort, secureSocket, cancellationToken);

            if (!string.IsNullOrWhiteSpace(options.SmtpUser))
            {
                await client.AuthenticateAsync(
                    options.SmtpUser,
                    options.SmtpPassword ?? string.Empty,
                    cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(quit: true, cancellationToken);

            logger.LogInformation("E-mail enviado para {To} via {Host}:{Port}", to, options.SmtpHost, options.SmtpPort);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao enviar e-mail SMTP para {To}", to);
            throw;
        }
    }

    private static SecureSocketOptions ResolverSeguranca(EmailOptions options)
    {
        if (string.Equals(options.Security, "None", StringComparison.OrdinalIgnoreCase))
            return SecureSocketOptions.None;

        if (string.Equals(options.Security, "StartTls", StringComparison.OrdinalIgnoreCase))
            return SecureSocketOptions.StartTls;

        if (string.Equals(options.Security, "SslOnConnect", StringComparison.OrdinalIgnoreCase))
            return SecureSocketOptions.SslOnConnect;

        // Auto: convenção usual dos provedores
        return options.SmtpPort == 465
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;
    }
}
