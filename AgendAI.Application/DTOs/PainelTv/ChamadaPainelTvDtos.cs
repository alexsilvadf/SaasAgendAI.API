namespace AgendAI.Application.DTOs.PainelTv;

public sealed class ChamadaPainelTvDto
{
    public string PacienteNome { get; set; } = string.Empty;

    public string ProfissionalNome { get; set; } = string.Empty;

    public Guid? ProfissionalId { get; set; }

    public Guid? AgendamentoId { get; set; }

    public Guid? PacienteId { get; set; }

    public string? Horario { get; set; }

    public long Timestamp { get; set; }
}

public sealed class ProximoPacientePainelTvDto
{
    public Guid AgendamentoId { get; set; }

    public string PacienteNome { get; set; } = string.Empty;

    public string ProfissionalNome { get; set; } = string.Empty;

    public string Horario { get; set; } = string.Empty;
}

public sealed class PublicarChamadaPainelTvRequest
{
    public string PacienteNome { get; set; } = string.Empty;

    public string? ProfissionalNome { get; set; }

    public Guid? ProfissionalId { get; set; }

    public Guid? AgendamentoId { get; set; }

    public Guid? PacienteId { get; set; }

    public string? Horario { get; set; }

    public long Timestamp { get; set; }
}
