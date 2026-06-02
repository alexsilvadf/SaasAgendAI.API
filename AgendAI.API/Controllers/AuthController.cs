using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendAI.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponse>> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.SolicitarRecuperacaoSenhaAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponse>> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RedefinirSenhaAsync(request, cancellationToken);
        return Ok(response);
    }
}
