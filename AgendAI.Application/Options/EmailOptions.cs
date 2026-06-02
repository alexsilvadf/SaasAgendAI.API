namespace AgendAI.Application.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>Quando true e SMTP estiver configurado, envia e-mails reais.</summary>
    public bool Enabled { get; set; }

    public string FromAddress { get; set; } = "noreply@agendai.local";

    public string FromName { get; set; } = "AgendAI";

    public string? SmtpHost { get; set; }

    public int SmtpPort { get; set; } = 587;

    public string? SmtpUser { get; set; }

    public string? SmtpPassword { get; set; }

    /// <summary>Auto, None, StartTls ou SslOnConnect. Auto: porta 465 = SSL, demais = StartTls.</summary>
    public string Security { get; set; } = "Auto";

    public bool IsConfigured =>
        Enabled
        && !string.IsNullOrWhiteSpace(SmtpHost)
        && !string.IsNullOrWhiteSpace(FromAddress)
        && !string.IsNullOrWhiteSpace(SmtpUser)
        && !string.IsNullOrWhiteSpace(SmtpPassword);
}
