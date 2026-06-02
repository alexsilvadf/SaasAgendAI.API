using AgendAI.Domain.Abstractions;

namespace AgendAI.Domain.Entities;

public class TokenRecuperacaoSenha : Entity, ITenantOwned
{
    public Guid TenantId { get; set; }

    public Guid UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }

    public bool Utilizado { get; set; }
}
