using AgendAI.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AgendAI.Infra.Tenancy;

public sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    private readonly Guid? _tenantId = ResolveTenantId(httpContextAccessor.HttpContext?.User);

    public Guid TenantId => _tenantId ?? Guid.Empty;

    public bool IsResolved => _tenantId.HasValue && _tenantId.Value != Guid.Empty;

    private static Guid? ResolveTenantId(ClaimsPrincipal? user)
    {
        var rawTenant = user?.FindFirstValue("tenant_id");
        return Guid.TryParse(rawTenant, out var tenantId) ? tenantId : null;
    }
}

