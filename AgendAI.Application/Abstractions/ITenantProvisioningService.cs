using AgendAI.Application.DTOs.Tenants;

namespace AgendAI.Application.Abstractions;

public interface ITenantProvisioningService
{
    Task<RegisterTenantResponse> RegisterAsync(
        RegisterTenantRequest request,
        CancellationToken cancellationToken = default);
}

