using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.PainelTv;
using AgendAI.Domain.Entities;
using AgendAI.Domain.Enums;
using AgendAI.Domain.Exceptions;
using AgendAI.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infra.Services;

public sealed class PainelTvService(AgendAiDbContext db) : IPainelTvService
{
    private const int RegistroUnicoId = 1;

    public async Task<ChamadaPainelTvDto?> ObterChamadaAtualAsync(CancellationToken cancellationToken = default)
    {
        var chamada = await db.ChamadasPainelTvAtual
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == RegistroUnicoId, cancellationToken);

        if (chamada is null || string.IsNullOrWhiteSpace(chamada.PacienteNome))
            return null;

        var profissionalNome = chamada.ProfissionalNome;
        if (chamada.ProfissionalId.HasValue)
        {
            var nomeAtual = await db.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == chamada.ProfissionalId.Value)
                .Select(u => u.Nome)
                .FirstOrDefaultAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(nomeAtual))
                profissionalNome = nomeAtual;
        }

        return ToDto(chamada, profissionalNome);
    }

    public async Task<ChamadaPainelTvDto> PublicarChamadaAsync(
        PublicarChamadaPainelTvRequest request,
        CancellationToken cancellationToken = default)
    {
        var pacienteNome = request.PacienteNome.Trim();

        if (string.IsNullOrWhiteSpace(pacienteNome))
            throw new ValidationException(nameof(request.PacienteNome), "Nome do paciente é obrigatório.");

        var (profissionalId, profissionalNome) = await ResolverProfissionalDoAtendimentoAsync(
            request,
            cancellationToken);

        var horario = string.IsNullOrWhiteSpace(request.Horario) ? null : request.Horario.Trim();
        var timestamp = request.Timestamp > 0
            ? request.Timestamp
            : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var chamada = await db.ChamadasPainelTvAtual
            .FirstOrDefaultAsync(c => c.Id == RegistroUnicoId, cancellationToken);

        if (chamada is null)
        {
            chamada = new ChamadaPainelTvAtual { Id = RegistroUnicoId };
            db.ChamadasPainelTvAtual.Add(chamada);
        }

        chamada.PacienteNome = pacienteNome;
        chamada.ProfissionalNome = profissionalNome;
        chamada.ProfissionalId = profissionalId;
        chamada.AgendamentoId = request.AgendamentoId;
        chamada.PacienteId = request.PacienteId;
        chamada.Horario = horario;
        chamada.Timestamp = timestamp;

        await db.SaveChangesAsync(cancellationToken);

        return ToDto(chamada);
    }

    public async Task LimparChamadaRelacionadaAsync(
        Guid? agendamentoId,
        Guid? pacienteId,
        CancellationToken cancellationToken = default)
    {
        if (!agendamentoId.HasValue && !pacienteId.HasValue)
            return;

        var chamada = await db.ChamadasPainelTvAtual
            .FirstOrDefaultAsync(c => c.Id == RegistroUnicoId, cancellationToken);

        if (chamada is null)
            return;

        var deveLimpar =
            (agendamentoId.HasValue && chamada.AgendamentoId == agendamentoId) ||
            (pacienteId.HasValue && chamada.PacienteId == pacienteId);

        if (!deveLimpar)
            return;

        chamada.PacienteNome = string.Empty;
        chamada.ProfissionalNome = string.Empty;
        chamada.ProfissionalId = null;
        chamada.AgendamentoId = null;
        chamada.PacienteId = null;
        chamada.Horario = null;
        chamada.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProximoPacientePainelTvDto>> ListarProximosPacientesAsync(
        int quantidade = 5,
        CancellationToken cancellationToken = default)
    {
        quantidade = Math.Clamp(quantidade, 1, 20);

        var agoraBrasil = ObterAgoraBrasil();
        var hoje = DateOnly.FromDateTime(agoraBrasil);
        var horaAtual = TimeOnly.FromDateTime(agoraBrasil);

        var chamada = await db.ChamadasPainelTvAtual
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == RegistroUnicoId, cancellationToken);

        var query = db.Agendamentos
            .AsNoTracking()
            .Include(a => a.Paciente)
            .Include(a => a.Profissional)
            .Where(a => a.Data == hoje)
            .Where(a => a.Status == StatusAgendamento.Agendado)
            .Where(a => a.HoraInicio >= horaAtual)
            .Where(a => !db.Atendimentos.Any(at =>
                at.Data == hoje &&
                (at.AgendamentoId == a.Id ||
                 (at.PacienteId == a.PacienteId &&
                  at.ProfissionalId == a.ProfissionalId &&
                  at.Hora == a.HoraInicio))));

        if (chamada?.AgendamentoId is Guid agendamentoChamado)
            query = query.Where(a => a.Id != agendamentoChamado);

        if (chamada?.PacienteId is Guid pacienteChamado)
            query = query.Where(a => a.PacienteId != pacienteChamado);

        return await query
            .OrderBy(a => a.HoraInicio)
            .Take(quantidade)
            .Select(a => new ProximoPacientePainelTvDto
            {
                AgendamentoId = a.Id,
                PacienteNome = a.Paciente.Nome,
                ProfissionalNome = a.Profissional.Nome,
                Horario = a.HoraInicio.ToString("HH:mm")
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<(Guid ProfissionalId, string ProfissionalNome)> ResolverProfissionalDoAtendimentoAsync(
        PublicarChamadaPainelTvRequest request,
        CancellationToken cancellationToken)
    {
        if (request.AgendamentoId.HasValue)
        {
            var agendamento = await db.Agendamentos
                .AsNoTracking()
                .Include(a => a.Profissional)
                .FirstOrDefaultAsync(a => a.Id == request.AgendamentoId.Value, cancellationToken);

            if (agendamento is null)
                throw new NotFoundException("Agendamento", request.AgendamentoId.Value);

            return (agendamento.ProfissionalId, agendamento.Profissional.Nome);
        }

        if (request.ProfissionalId.HasValue)
        {
            var profissional = await db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Id == request.ProfissionalId.Value && u.Ativo,
                    cancellationToken);

            if (profissional is null)
                throw new NotFoundException("Profissional", request.ProfissionalId.Value);

            return (profissional.Id, profissional.Nome);
        }

        throw new ValidationException(
            nameof(request.ProfissionalId),
            "Informe o profissional do agendamento (profissionalId ou agendamentoId).");
    }

    private static DateTime ObterAgoraBrasil()
    {
        try
        {
            var fusoId = OperatingSystem.IsWindows()
                ? "E. South America Standard Time"
                : "America/Sao_Paulo";
            var fuso = TimeZoneInfo.FindSystemTimeZoneById(fusoId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, fuso);
        }
        catch
        {
            return DateTime.Now;
        }
    }

    private static ChamadaPainelTvDto ToDto(ChamadaPainelTvAtual chamada, string? profissionalNome = null) =>
        new()
        {
            PacienteNome = chamada.PacienteNome,
            ProfissionalNome = profissionalNome ?? chamada.ProfissionalNome,
            ProfissionalId = chamada.ProfissionalId,
            AgendamentoId = chamada.AgendamentoId,
            PacienteId = chamada.PacienteId,
            Horario = chamada.Horario,
            Timestamp = chamada.Timestamp
        };
}
