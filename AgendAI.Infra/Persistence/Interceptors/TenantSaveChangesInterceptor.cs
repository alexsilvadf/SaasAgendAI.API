using AgendAI.Application.Abstractions;
using AgendAI.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AgendAI.Infra.Persistence.Interceptors;

public sealed class TenantSaveChangesInterceptor(ITenantContext tenantContext) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyTenantRules(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyTenantRules(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyTenantRules(DbContext? context)
    {
        if (context is null || !tenantContext.IsResolved)
            return;

        var entries = context.ChangeTracker.Entries<ITenantOwned>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            EnsureTenant(entry);
        }
    }

    private void EnsureTenant(EntityEntry<ITenantOwned> entry)
    {
        if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
            entry.Entity.TenantId = tenantContext.TenantId;

        if (entry.Entity.TenantId != tenantContext.TenantId)
        {
            throw new InvalidOperationException(
                $"TenantId inválido para {entry.Metadata.ClrType.Name}: " +
                $"esperado {tenantContext.TenantId}, recebido {entry.Entity.TenantId}.");
        }
    }
}

