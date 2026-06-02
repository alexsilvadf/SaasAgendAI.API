using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Agenda;
using AgendAI.Domain.Enums;
using AgendAI.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infra.Services;

public sealed class AgendaService(AgendAiDbContext db) : IAgendaService
{
    public async Task<IReadOnlyList<ProfessionalDto>> ListarProfissionaisAsync(
        CancellationToken cancellationToken = default)
    {
        return await db.Usuarios
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Dentista && u.Ativo)
            .OrderBy(u => u.Nome)
            .Select(u => new ProfessionalDto
            {
                Id = u.Id,
                Name = u.Nome,
                Specialty = u.Especialidade ?? string.Empty
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DayScheduleDto>> MontarGradeAsync(
        DateOnly data,
        Guid? profissionalId,
        UserRole role,
        Guid? usuarioLogadoId,
        CancellationToken cancellationToken = default)
    {
        if (role == UserRole.Dentista && usuarioLogadoId.HasValue)
            profissionalId = usuarioLogadoId;

        var config = await db.ConfiguracoesClinica.AsNoTracking().FirstAsync(cancellationToken);
        var profissionais = await db.Usuarios
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Dentista && u.Ativo)
            .Where(u => !profissionalId.HasValue || u.Id == profissionalId)
            .ToListAsync(cancellationToken);

        var agendamentos = await db.Agendamentos
            .AsNoTracking()
            .Include(a => a.Paciente)
            .Include(a => a.Procedimento)
            .Where(a => a.Data == data && (a.Status == StatusAgendamento.Agendado || a.Status == StatusAgendamento.NaoCompareceu))
            .Where(a => !profissionalId.HasValue || a.ProfissionalId == profissionalId)
            .ToListAsync(cancellationToken);

        var bloqueios = await db.BloqueiosAgenda
            .AsNoTracking()
            .Where(b => b.Data == data)
            .Where(b => !profissionalId.HasValue || b.ProfissionalId == profissionalId)
            .ToListAsync(cancellationToken);

        var atendimentos = await db.Atendimentos
            .AsNoTracking()
            .Include(a => a.Paciente)
            .Include(a => a.Procedimento)
            .Where(a => a.Data == data)
            .Where(a => !profissionalId.HasValue || a.ProfissionalId == profissionalId)
            .ToListAsync(cancellationToken);

        var slotsBase = GerarSlots(config.HoraAbertura, config.HoraFechamento, config.IntervaloMinutos);
        var resultado = new List<DayScheduleDto>();

        foreach (var prof in profissionais)
        {
            var slots = slotsBase.Select(s => new AgendaSlotDto
            {
                Start = s.Start,
                End = s.End,
                Status = SlotStatus.Livre.ToJsonValue()
            }).ToList();

            foreach (var bloqueio in bloqueios.Where(b => b.ProfissionalId == prof.Id))
            {
                AplicarIntervalo(slots, bloqueio.HoraInicio, bloqueio.HoraFim, SlotStatus.Indisponivel.ToJsonValue(), bloqueio.Motivo);
            }

            foreach (var ag in agendamentos.Where(a => a.ProfissionalId == prof.Id))
            {
                if (ag.Status == StatusAgendamento.NaoCompareceu)
                {
                    AplicarIntervalo(
                        slots,
                        ag.HoraInicio,
                        ag.HoraFim,
                        SlotStatus.NaoCompareceu.ToJsonValue(),
                        "Não compareceu",
                        ag.Paciente.Nome,
                        ag.Id);
                    continue;
                }

                AplicarIntervalo(slots, ag.HoraInicio, ag.HoraFim, SlotStatus.Ocupado.ToJsonValue(), ag.Procedimento.Nome, ag.Paciente.Nome, ag.Id);
            }

            foreach (var at in atendimentos.Where(a => a.ProfissionalId == prof.Id))
            {
                var fim = at.Hora.AddMinutes(config.IntervaloMinutos);
                AplicarIntervalo(
                    slots,
                    at.Hora,
                    fim,
                    SlotStatus.Ocupado.ToJsonValue(),
                    at.Procedimento.Nome,
                    at.Paciente.Nome,
                    at.AgendamentoId,
                    at.Id,
                    pendentePagamento: !at.Pago,
                    atendimentoPago: at.Pago);
            }

            resultado.Add(new DayScheduleDto
            {
                Date = data.ToString("yyyy-MM-dd"),
                ProfessionalId = prof.Id,
                Slots = slots
            });
        }

        return resultado;
    }

    private static List<(string Start, string End)> GerarSlots(TimeOnly abertura, TimeOnly fechamento, int intervaloMinutos)
    {
        var slots = new List<(string Start, string End)>();
        var atual = abertura;

        while (atual.AddMinutes(intervaloMinutos) <= fechamento)
        {
            var fim = atual.AddMinutes(intervaloMinutos);
            slots.Add((atual.ToString("HH:mm"), fim.ToString("HH:mm")));
            atual = fim;
        }

        return slots;
    }

    private static void AplicarIntervalo(
        List<AgendaSlotDto> slots,
        TimeOnly inicio,
        TimeOnly fim,
        string status,
        string? detail = null,
        string? patientName = null,
        Guid? agendamentoId = null,
        Guid? atendimentoId = null,
        bool pendentePagamento = false,
        bool atendimentoPago = false)
    {
        foreach (var slot in slots)
        {
            var slotInicio = TimeOnly.Parse(slot.Start);
            if (slotInicio >= inicio && slotInicio < fim)
            {
                slot.Status = status;
                slot.Detail = detail;
                slot.PatientName = patientName;
                slot.AgendamentoId = agendamentoId;

                if (atendimentoId.HasValue)
                    slot.AtendimentoId = atendimentoId;

                if (pendentePagamento)
                    slot.PendentePagamento = true;

                if (atendimentoPago)
                    slot.AtendimentoPago = true;
            }
        }
    }
}
