namespace AgendAI.Domain.Entities;

public sealed class Tenant
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; }
}

