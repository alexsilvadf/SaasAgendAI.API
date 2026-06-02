using AgendAI.API.Extensions;
using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.PainelTv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendAI.API.Controllers;

[ApiController]
[Route("api/v1/painel-tv")]
public sealed class PainelTvController(IPainelTvService painelTvService) : ControllerBase
{
    [HttpGet("chamada-atual")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterChamadaAtual(CancellationToken cancellationToken)
    {
        var chamada = await painelTvService.ObterChamadaAtualAsync(cancellationToken);
        return Ok(chamada);
    }

    [HttpGet("proximos-pacientes")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarProximosPacientes(
        [FromQuery] int quantidade = 5,
        CancellationToken cancellationToken = default)
    {
        var itens = await painelTvService.ListarProximosPacientesAsync(quantidade, cancellationToken);
        return Ok(itens);
    }

    [HttpPost("chamada-atual")]
    [Authorize]
    public async Task<IActionResult> PublicarChamada(
        [FromBody] PublicarChamadaPainelTvRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.HasPermission("atendimento:create"))
            return Forbid();

        var chamada = await painelTvService.PublicarChamadaAsync(request, cancellationToken);
        return Ok(chamada);
    }
}
