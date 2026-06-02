namespace AgendAI.Application.DTOs.Auth;

public sealed class ForgotPasswordRequest
{
    /// <summary>Login ou e-mail cadastrado do usuário.</summary>
    public string Identificador { get; set; } = string.Empty;
}
