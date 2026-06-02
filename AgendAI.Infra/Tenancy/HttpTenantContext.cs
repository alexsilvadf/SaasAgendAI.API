using System.Security.Claims;
using AgendAI.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace AgendAI.Infra.Tenancy;

public sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public Guid TenantId => ResolveTenantId() ?? Guid.Empty;

    public bool IsResolved
    {
        get
        {
            var tenantId = ResolveTenantId();
            return tenantId.HasValue && tenantId.Value != Guid.Empty;
        }
    }

    private Guid? ResolveTenantId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        foreach (var claimType in new[] { "tenant_id", "TenantId", "tenantId" })
        {
            var raw = user.FindFirstValue(claimType);
            if (Guid.TryParse(raw, out var tenantId) && tenantId != Guid.Empty)
                return tenantId;
        }

        return null;
    }
}
