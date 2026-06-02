namespace AgendAI.Application.DTOs.Auth;

public sealed class ForgotPasswordRequest
{
    /// <summary>Slug do tenant/clínica.</summary>
    public string TenantSlug { get; set; } = string.Empty;

    /// <summary>Login ou e-mail cadastrado do usuário.</summary>
    public string Identificador { get; set; } = string.Empty;
}
