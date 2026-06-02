using AgendAI.Application.Abstractions;
using AgendAI.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgendAI.Infra.Email;

public static class EmailServiceRegistration
{
    public static IServiceCollection AddAgendAIEmail(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

        var emailOptions = configuration.GetSection(EmailOptions.SectionName).Get<EmailOptions>() ?? new EmailOptions();

        if (emailOptions.IsConfigured)
        {
            services.AddScoped<IEmailSender, SmtpEmailSender>();
        }
        else
        {
            services.AddScoped<IEmailSender, LoggingEmailSender>();
        }

        return services;
    }

    public static void LogEmailMode(this IServiceProvider services)
    {
        var options = services.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailOptions>>().Value;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("AgendAI.Email");

        if (options.IsConfigured)
        {
            logger.LogInformation(
                "E-mail SMTP ativo: {Host}:{Port} ({Security}), remetente {From}",
                options.SmtpHost,
                options.SmtpPort,
                options.Security,
                options.FromAddress);
            return;
        }

        if (options.Enabled)
        {
            logger.LogWarning(
                "Email:Enabled=true, mas SMTP incompleto (Host, User, Password ou From). Usando log em vez de envio real.");
        }
        else
        {
            logger.LogInformation("E-mail em modo desenvolvimento (links gravados no log). Defina Email:Enabled e credenciais SMTP para envio real.");
        }
    }
}
