using AgendAI.Domain.Abstractions;
using AgendAI.Domain.Enums;

namespace AgendAI.Domain.Entities;

public class BloqueioAgenda : Entity, ITenantOwned
{
    public Guid TenantId { get; set; }

    public Guid? ProfissionalId { get; set; }

    public Usuario? Profissional { get; set; }

    public DateOnly Data { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFim { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public TipoBloqueioAgenda Tipo { get; set; }
}
