namespace AgendAI.Application.DTOs.Auth;

public sealed class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;

    public string NovaSenha { get; set; } = string.Empty;

    public string ConfirmarSenha { get; set; } = string.Empty;
}
