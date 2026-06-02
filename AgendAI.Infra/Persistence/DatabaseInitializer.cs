using AgendAI.Infra.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AgendAI.Infra.Persistence;

public static class DatabaseInitializer
{
    private const string ProductVersion = "8.0.11";

    public static async Task InitializeAsync(IServiceProvider services, IHostEnvironment environment)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AgendAiDbContext>>();
        var db = scope.ServiceProvider.GetRequiredService<AgendAiDbContext>();

        try
        {
            if (db.Database.IsInMemory())
            {
                await db.Database.EnsureCreatedAsync();

                if (!await db.Usuarios.AnyAsync())
                    await AgendAiDbSeeder.SeedAsync(db);

                logger.LogInformation("Banco em memória pronto.");
                return;
            }

            await ApplyMigrationsAsync(db, logger);
            await AgendAiDbSeeder.SeedAsync(db);

            logger.LogInformation(
                await db.Usuarios.AnyAsync()
                    ? "Migrações aplicadas; dados iniciais verificados."
                    : "Migrações aplicadas e usuários iniciais criados.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao inicializar o banco de dados.");
            throw;
        }
    }

    private static async Task ApplyMigrationsAsync(AgendAiDbContext db, ILogger logger)
    {
        var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
        if (pending.Count == 0)
            return;

        var applied = await db.Database.GetAppliedMigrationsAsync();
        if (!applied.Any() && await LegacySchemaExistsAsync(db))
        {
            await BaselineMigrationsAsync(db, pending, logger);
            return;
        }

        try
        {
            await db.Database.MigrateAsync();
        }
        catch (PostgresException ex) when (ex.SqlState == "42P07") // duplicate_table
        {
            if (!await LegacySchemaExistsAsync(db))
                throw;

            logger.LogWarning(
                ex,
                "Migração falhou porque o esquema já existe; registrando migrações pendentes como aplicadas.");
            await BaselineMigrationsAsync(db, pending, logger);
        }
    }

    private static async Task<bool> LegacySchemaExistsAsync(AgendAiDbContext db) =>
        await db.Database.SqlQueryRaw<int>(
                """
                SELECT COUNT(*)::int AS "Value"
                FROM information_schema.tables
                WHERE table_schema = 'public' AND table_name = 'Usuarios'
                """)
            .SingleAsync() > 0;

    private static async Task BaselineMigrationsAsync(
        AgendAiDbContext db,
        IReadOnlyList<string> migrationIds,
        ILogger logger)
    {
        foreach (var migrationId in migrationIds)
        {
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                 SELECT {migrationId}, {ProductVersion}
                 WHERE NOT EXISTS (
                     SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = {migrationId}
                 )
                 """);
        }

        logger.LogWarning(
            "Esquema legado detectado; {Count} migração(ões) registrada(s) sem recriar tabelas.",
            migrationIds.Count);
    }
}
