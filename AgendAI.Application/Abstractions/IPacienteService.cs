using AgendAI.Application.DTOs.Pacientes;

namespace AgendAI.Application.Abstractions;

public interface IPacienteService
{
    Task<IReadOnlyList<PacienteResumoDto>> ListarAsync(string? nome, CancellationToken cancellationToken = default);

    Task<PacienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PacienteDto> CriarAsync(SalvarPacienteRequest request, CancellationToken cancellationToken = default);

    Task<PacienteDto> AtualizarAsync(Guid id, SalvarPacienteRequest request, CancellationToken cancellationToken = default);

    Task<PacienteDto> AtualizarAtivoAsync(Guid id, bool ativo, CancellationToken cancellationToken = default);
}
