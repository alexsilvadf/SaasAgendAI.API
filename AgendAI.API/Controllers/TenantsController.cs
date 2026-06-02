using AgendAI.Application.Abstractions;
using AgendAI.Application.DTOs.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendAI.API.Controllers;

[ApiController]
[Route("api/v1/tenants")]
public sealed class TenantsController(ITenantProvisioningService tenantProvisioningService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterTenantResponse>> Register(
        [FromBody] RegisterTenantRequest request,
        CancellationToken cancellationToken)
    {
        var response = await tenantProvisioningService.RegisterAsync(request, cancellationToken);
        return Ok(response);
    }
}

