using AgendAI.Application.DTOs.PainelTv;

namespace AgendAI.Application.Abstractions;

public interface IPainelTvService
{
    Task<ChamadaPainelTvDto?> ObterChamadaAtualAsync(CancellationToken cancellationToken = default);

    Task<ChamadaPainelTvDto> PublicarChamadaAsync(
        PublicarChamadaPainelTvRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProximoPacientePainelTvDto>> ListarProximosPacientesAsync(
        int quantidade = 5,
        CancellationToken cancellationToken = default);

    Task LimparChamadaRelacionadaAsync(
        Guid? agendamentoId,
        Guid? pacienteId,
        CancellationToken cancellationToken = default);
}
