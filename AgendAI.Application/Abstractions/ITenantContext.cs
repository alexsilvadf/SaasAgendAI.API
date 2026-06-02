namespace AgendAI.Application.Abstractions;

public interface ITenantContext
{
    Guid TenantId { get; }

    bool IsResolved { get; }
}

