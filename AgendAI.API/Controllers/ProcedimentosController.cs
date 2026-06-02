using AgendAI.API.Extensions;
using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Procedimentos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendAI.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/procedimentos")]
public sealed class ProcedimentosController(IProcedimentoService procedimentoService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Cadastrar(
        [FromBody] CadastrarProcedimentoRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.HasPermission("procedimentos:edit"))
            return Forbid();

        var procedimento = await procedimentoService.CadastrarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { }, procedimento);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        if (!User.HasPermission("procedimentos:view"))
            return Forbid();

        return Ok(await procedimentoService.ListarAsync(cancellationToken));
    }

    [HttpGet("ativos")]
    public async Task<IActionResult> ListarAtivos(CancellationToken cancellationToken)
    {
        if (!User.HasPermission("agendamento:create") && !User.HasPermission("procedimentos:view"))
            return Forbid();

        return Ok(await procedimentoService.ListarAtivosAsync(cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromBody] AtualizarProcedimentoRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.HasPermission("procedimentos:edit"))
            return Forbid();

        var atualizado = await procedimentoService.AtualizarAsync(id, request, cancellationToken);
        return Ok(atualizado);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        if (!User.HasPermission("procedimentos:edit"))
            return Forbid();

        await procedimentoService.ExcluirAsync(id, cancellationToken);
        return NoContent();
    }
}
