using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infra.Persistence;

/// <summary>
/// Aloca IDs inteiros para tabelas que migraram de singleton (Id fixo) para identity por tenant.
/// Evita colisão quando a sequence do PostgreSQL ainda aponta para 1.
/// </summary>
internal static class IntEntityIdAllocator
{
    public static async Task<int> NextChamadaPainelTvIdAsync(
        AgendAiDbContext db,
        CancellationToken cancellationToken = default)
    {
        var max = await db.ChamadasPainelTvAtual
            .IgnoreQueryFilters()
            .MaxAsync(c => (int?)c.Id, cancellationToken);

        return (max ?? 0) + 1;
    }

    public static async Task<int> NextConfiguracaoClinicaIdAsync(
        AgendAiDbContext db,
        CancellationToken cancellationToken = default)
    {
        var max = await db.ConfiguracoesClinica
            .IgnoreQueryFilters()
            .MaxAsync(c => (int?)c.Id, cancellationToken);

        return (max ?? 0) + 1;
    }
}
