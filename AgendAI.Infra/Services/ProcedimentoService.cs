using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Procedimentos;
using AgendAI.Domain.Entities;
using AgendAI.Domain.Enums;
using AgendAI.Domain.Exceptions;
using AgendAI.Infra.Mapping;
using AgendAI.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infra.Services;

public sealed class ProcedimentoService(AgendAiDbContext db) : IProcedimentoService
{
    public async Task<ProcedimentoDto> CadastrarAsync(
        CadastrarProcedimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        var procedimento = new Procedimento
        {
            Nome = request.Nome.Trim(),
            Valor = request.Valor,
            Status = StatusProcedimento.Ativo,
            CriadoEm = DateTime.UtcNow
        };

        db.Procedimentos.Add(procedimento);
        await db.SaveChangesAsync(cancellationToken);

        return EntityMapper.ToDto(procedimento);
    }

    public async Task<IReadOnlyList<ProcedimentoDto>> ListarAtivosAsync(
        CancellationToken cancellationToken = default) =>
        await ListarInternoAsync(ativo: true, cancellationToken);

    public async Task<IReadOnlyList<ProcedimentoDto>> ListarAsync(
        CancellationToken cancellationToken = default) =>
        await ListarInternoAsync(ativo: null, cancellationToken);

    private async Task<IReadOnlyList<ProcedimentoDto>> ListarInternoAsync(
        bool? ativo,
        CancellationToken cancellationToken)
    {
        var query = db.Procedimentos.AsNoTracking().AsQueryable();

        if (ativo == true)
            query = query.Where(p => p.Status == StatusProcedimento.Ativo);

        var itens = await query.OrderBy(p => p.Nome).ToListAsync(cancellationToken);
        return itens.Select(EntityMapper.ToDto).ToList();
    }

    public async Task<ProcedimentoDto> AtualizarAsync(
        Guid id,
        AtualizarProcedimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        var procedimento = await db.Procedimentos
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException("Procedimento", id);

        procedimento.Nome = request.Nome.Trim();
        procedimento.Valor = request.Valor;
        procedimento.Status = EnumExtensions.FromJsonValue<StatusProcedimento>(request.Status);

        await db.SaveChangesAsync(cancellationToken);
        return EntityMapper.ToDto(procedimento);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var procedimento = await db.Procedimentos
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException("Procedimento", id);

        var emUso = await db.Agendamentos.AnyAsync(a => a.ProcedimentoId == id, cancellationToken)
            || await db.Atendimentos.AnyAsync(a => a.ProcedimentoId == id, cancellationToken);

        if (emUso)
        {
            throw new ConflictException(
                "Não é possível excluir: procedimento vinculado a agendamentos ou atendimentos. Inative-o em vez disso.");
        }

        db.Procedimentos.Remove(procedimento);
        await db.SaveChangesAsync(cancellationToken);
    }
}
