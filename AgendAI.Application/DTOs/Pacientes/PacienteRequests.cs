namespace AgendAI.Application.DTOs.Pacientes;

public sealed class SalvarPacienteRequest
{
    public string Nome { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string DataNascimento { get; set; } = string.Empty;

    public string Sexo { get; set; } = string.Empty;

    public string EstadoCivil { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Cep { get; set; } = string.Empty;

    public string Logradouro { get; set; } = string.Empty;

    public string Numero { get; set; } = string.Empty;

    public string Complemento { get; set; } = string.Empty;

    public string Bairro { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Uf { get; set; } = string.Empty;

    public string TipoSanguineo { get; set; } = string.Empty;

    public AnamneseDto Anamnese { get; set; } = new();

    public bool Ativo { get; set; } = true;
}

public sealed class AtualizarPacienteAtivoRequest
{
    public bool Ativo { get; set; }
}
