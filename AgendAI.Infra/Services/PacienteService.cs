using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Pacientes;
using AgendAI.Domain.Entities;
using AgendAI.Domain.Enums;
using AgendAI.Domain.Exceptions;
using AgendAI.Infra.Mapping;
using AgendAI.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infra.Services;

public sealed class PacienteService(AgendAiDbContext db) : IPacienteService
{
    public async Task<IReadOnlyList<PacienteResumoDto>> ListarAsync(
        string? nome,
        CancellationToken cancellationToken = default)
    {
        var query = db.Pacientes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLowerInvariant();
            var cpfDigits = SomenteDigitos(nome);
            query = query.Where(p =>
                p.Nome.ToLower().Contains(termo)
                || (cpfDigits.Length > 0 && p.Cpf.Contains(cpfDigits))
                || p.Telefone.Contains(termo));
        }

        var pacientes = await query.OrderBy(p => p.Nome).ToListAsync(cancellationToken);
        return pacientes.Select(EntityMapper.ToResumo).ToList();
    }

    public async Task<PacienteDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var paciente = await db.Pacientes
            .AsNoTracking()
            .Include(p => p.Anamnese)
            .Include(p => p.Historicos)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return paciente is null ? null : EntityMapper.ToDto(paciente);
    }

    public async Task<PacienteDto> CriarAsync(
        SalvarPacienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var cpf = NormalizarCpf(request.Cpf);
        await GarantirCpfUnicoAsync(cpf, null, cancellationToken);

        var now = DateTime.UtcNow;
        var paciente = new Paciente
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome.Trim(),
            Cpf = cpf,
            DataNascimento = ParseDataNascimento(request.DataNascimento),
            Sexo = EnumExtensions.FromJsonValue<Sexo>(request.Sexo),
            EstadoCivil = EnumExtensions.FromJsonValue<EstadoCivil>(request.EstadoCivil),
            Telefone = request.Telefone.Trim(),
            Email = request.Email.Trim(),
            Cep = SomenteDigitos(request.Cep),
            Logradouro = request.Logradouro.Trim(),
            Numero = request.Numero.Trim(),
            Complemento = request.Complemento.Trim(),
            Bairro = request.Bairro.Trim(),
            Cidade = request.Cidade.Trim(),
            Uf = request.Uf.Trim().ToUpperInvariant(),
            TipoSanguineo = EnumExtensions.FromJsonValue<TipoSanguineo>(request.TipoSanguineo),
            Ativo = request.Ativo,
            CriadoEm = now,
            AtualizadoEm = now,
            Anamnese = CriarAnamnese(Guid.Empty, request.Anamnese)
        };

        paciente.Anamnese!.PacienteId = paciente.Id;

        db.Pacientes.Add(paciente);
        await db.SaveChangesAsync(cancellationToken);

        return await ObterDtoCompletoAsync(paciente.Id, cancellationToken)
            ?? throw new InvalidOperationException("Paciente criado mas não encontrado.");
    }

    public async Task<PacienteDto> AtualizarAsync(
        Guid id,
        SalvarPacienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var paciente = await db.Pacientes
            .Include(p => p.Anamnese)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException("Paciente", id);

        var cpf = NormalizarCpf(request.Cpf);
        await GarantirCpfUnicoAsync(cpf, id, cancellationToken);

        paciente.Nome = request.Nome.Trim();
        paciente.Cpf = cpf;
        paciente.DataNascimento = ParseDataNascimento(request.DataNascimento);
        paciente.Sexo = EnumExtensions.FromJsonValue<Sexo>(request.Sexo);
        paciente.EstadoCivil = EnumExtensions.FromJsonValue<EstadoCivil>(request.EstadoCivil);
        paciente.Telefone = request.Telefone.Trim();
        paciente.Email = request.Email.Trim();
        paciente.Cep = SomenteDigitos(request.Cep);
        paciente.Logradouro = request.Logradouro.Trim();
        paciente.Numero = request.Numero.Trim();
        paciente.Complemento = request.Complemento.Trim();
        paciente.Bairro = request.Bairro.Trim();
        paciente.Cidade = request.Cidade.Trim();
        paciente.Uf = request.Uf.Trim().ToUpperInvariant();
        paciente.TipoSanguineo = EnumExtensions.FromJsonValue<TipoSanguineo>(request.TipoSanguineo);
        paciente.Ativo = request.Ativo;
        paciente.AtualizadoEm = DateTime.UtcNow;

        if (paciente.Anamnese is null)
        {
            paciente.Anamnese = CriarAnamnese(paciente.Id, request.Anamnese);
            db.PacienteAnamneses.Add(paciente.Anamnese);
        }
        else
        {
            AplicarAnamnese(paciente.Anamnese, request.Anamnese);
        }

        await db.SaveChangesAsync(cancellationToken);

        return await ObterDtoCompletoAsync(paciente.Id, cancellationToken)
            ?? throw new InvalidOperationException("Paciente atualizado mas não encontrado.");
    }

    public async Task<PacienteDto> AtualizarAtivoAsync(
        Guid id,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        var paciente = await db.Pacientes
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException("Paciente", id);

        paciente.Ativo = ativo;
        paciente.AtualizadoEm = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return await ObterDtoCompletoAsync(paciente.Id, cancellationToken)
            ?? throw new InvalidOperationException("Paciente não encontrado após atualização de status.");
    }

    private async Task<PacienteDto?> ObterDtoCompletoAsync(Guid id, CancellationToken cancellationToken)
    {
        var paciente = await db.Pacientes
            .AsNoTracking()
            .Include(p => p.Anamnese)
            .Include(p => p.Historicos)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return paciente is null ? null : EntityMapper.ToDto(paciente);
    }

    private async Task GarantirCpfUnicoAsync(string cpf, Guid? ignorarId, CancellationToken cancellationToken)
    {
        if (cpf.Length != 11)
            throw new ConflictException("CPF inválido. Informe 11 dígitos.");

        var existe = await db.Pacientes.AnyAsync(
            p => p.Cpf == cpf && (!ignorarId.HasValue || p.Id != ignorarId.Value),
            cancellationToken);

        if (existe)
            throw new ConflictException("CPF já cadastrado para outro paciente.");
    }

    private static DateOnly ParseDataNascimento(string data)
    {
        if (!DateOnly.TryParse(data, out var parsed))
            throw new ConflictException("Data de nascimento inválida. Use o formato yyyy-MM-dd.");

        return parsed;
    }

    private static string NormalizarCpf(string cpf)
    {
        var digits = SomenteDigitos(cpf);
        if (digits.Length != 11)
            throw new ConflictException("CPF inválido. Informe 11 dígitos.");

        return digits;
    }

    private static string SomenteDigitos(string value) =>
        new string(value.Where(char.IsDigit).ToArray());

    private static PacienteAnamnese CriarAnamnese(Guid pacienteId, AnamneseDto dto)
    {
        var anamnese = new PacienteAnamnese { PacienteId = pacienteId };
        AplicarAnamnese(anamnese, dto);
        return anamnese;
    }

    private static void AplicarAnamnese(PacienteAnamnese anamnese, AnamneseDto dto)
    {
        anamnese.TemDoencaCardiaca = dto.TemDoencaCardiaca;
        anamnese.TemDiabetes = dto.TemDiabetes;
        anamnese.TemHipertensao = dto.TemHipertensao;
        anamnese.TemCoagulopatia = dto.TemCoagulopatia;
        anamnese.TemAlergiaMedicamento = dto.TemAlergiaMedicamento;
        anamnese.AlergiaMedicamentoDesc = dto.AlergiaMedicamentoDesc.Trim();
        anamnese.TemAlergiaMaterial = dto.TemAlergiaMaterial;
        anamnese.AlergiaMaterialDesc = dto.AlergiaMaterialDesc.Trim();
        anamnese.UsaMedicamentoContinuo = dto.UsaMedicamentoContinuo;
        anamnese.MedicamentoContinuoDesc = dto.MedicamentoContinuoDesc.Trim();
        anamnese.EstaGravida = dto.EstaGravida;
        anamnese.Fumante = dto.Fumante;
        anamnese.ObservacoesGerais = dto.ObservacoesGerais.Trim();
    }
}
