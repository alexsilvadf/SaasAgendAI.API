namespace AgendAI.Domain.Abstractions;

public interface ITenantOwned
{
    Guid TenantId { get; set; }
}

