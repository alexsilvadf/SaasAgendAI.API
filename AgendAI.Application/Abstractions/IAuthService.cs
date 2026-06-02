using AgendAI.Application.DTOs.Auth;

namespace AgendAI.Application.Abstractions;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<MessageResponse> SolicitarRecuperacaoSenhaAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default);

    Task<MessageResponse> RedefinirSenhaAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default);
}
