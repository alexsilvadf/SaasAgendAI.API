using AgendAI.Application.Abstractions;

namespace AgendAI.Infra.Tenancy;

public sealed class NullTenantContext : ITenantContext
{
    public Guid TenantId => Guid.Empty;

    public bool IsResolved => false;
}

