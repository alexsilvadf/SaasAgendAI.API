using AgendAI.Domain.Abstractions;

namespace AgendAI.Domain.Entities;

/// <summary>
/// Chamada exibida no painel/TV da recepção (registro único, Id = 1).
/// </summary>
public class ChamadaPainelTvAtual : ITenantOwned
{
    public int Id { get; set; } = 1;

    public Guid TenantId { get; set; }

    public string PacienteNome { get; set; } = string.Empty;

    public string ProfissionalNome { get; set; } = string.Empty;

    public Guid? ProfissionalId { get; set; }

    public Guid? AgendamentoId { get; set; }

    public Guid? PacienteId { get; set; }

    public string? Horario { get; set; }

    public long Timestamp { get; set; }
}
