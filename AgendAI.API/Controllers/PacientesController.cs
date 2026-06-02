using AgendAI.API.Extensions;
using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Pacientes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendAI.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/pacientes")]
public sealed class PacientesController(IPacienteService pacienteService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? nome, CancellationToken cancellationToken)
    {
        if (!User.HasPermission("pacientes:view"))
            return Forbid();

        var itens = await pacienteService.ListarAsync(nome, cancellationToken);
        return Ok(itens);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        if (!User.HasPermission("pacientes:view"))
            return Forbid();

        var paciente = await pacienteService.ObterPorIdAsync(id, cancellationToken);
        return paciente is null ? NotFound() : Ok(paciente);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] SalvarPacienteRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.HasPermission("pacientes:edit"))
            return Forbid();

        var criado = await pacienteService.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromBody] SalvarPacienteRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.HasPermission("pacientes:edit"))
            return Forbid();

        var atualizado = await pacienteService.AtualizarAsync(id, request, cancellationToken);
        return Ok(atualizado);
    }

    [HttpPatch("{id:guid}/ativo")]
    public async Task<IActionResult> AtualizarAtivo(
        Guid id,
        [FromBody] AtualizarPacienteAtivoRequest request,
        CancellationToken cancellationToken)
    {
        if (!User.HasPermission("pacientes:edit"))
            return Forbid();

        var atualizado = await pacienteService.AtualizarAtivoAsync(id, request.Ativo, cancellationToken);
        return Ok(atualizado);
    }
}
