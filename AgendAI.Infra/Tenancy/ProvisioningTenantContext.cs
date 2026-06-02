using AgendAI.Application.Abstractions;

namespace AgendAI.Infra.Tenancy;

public sealed class ProvisioningTenantContext(Guid tenantId) : ITenantContext
{
    public Guid TenantId { get; } = tenantId;

    public bool IsResolved => TenantId != Guid.Empty;
}

