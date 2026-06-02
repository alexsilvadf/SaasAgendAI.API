using AgendAI.Application.DTOs.Procedimentos;

namespace AgendAI.Application.Abstractions;

public interface IProcedimentoService
{
    Task<ProcedimentoDto> CadastrarAsync(CadastrarProcedimentoRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProcedimentoDto>> ListarAtivosAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProcedimentoDto>> ListarAsync(CancellationToken cancellationToken = default);

    Task<ProcedimentoDto> AtualizarAsync(
        Guid id,
        AtualizarProcedimentoRequest request,
        CancellationToken cancellationToken = default);

    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
}
