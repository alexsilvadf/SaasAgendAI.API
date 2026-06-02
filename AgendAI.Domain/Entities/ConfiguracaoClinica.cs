using AgendAI.Domain.Abstractions;

namespace AgendAI.Domain.Entities;

public class ConfiguracaoClinica : ITenantOwned
{
    public int Id { get; set; } = 1;

    public Guid TenantId { get; set; }

    public TimeOnly HoraAbertura { get; set; }

    public TimeOnly HoraFechamento { get; set; }

    public int IntervaloMinutos { get; set; } = 30;
}
