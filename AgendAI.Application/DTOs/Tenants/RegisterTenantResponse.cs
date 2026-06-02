namespace AgendAI.Application.DTOs.Tenants;

public sealed class RegisterTenantResponse
{
    public Guid TenantId { get; set; }

    public string TenantSlug { get; set; } = string.Empty;

    public string TenantNome { get; set; } = string.Empty;

    public Guid AdminUserId { get; set; }
}

