using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendAI.Infra.Persistence.Migrations;

/// <inheritdoc />
public partial class SyncSingletonTableIdentitySequences : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            SELECT setval(
                pg_get_serial_sequence('"PainelTvChamadaAtual"', 'Id'),
                COALESCE((SELECT MAX("Id") FROM "PainelTvChamadaAtual"), 1),
                true);

            SELECT setval(
                pg_get_serial_sequence('"ConfiguracoesClinica"', 'Id'),
                COALESCE((SELECT MAX("Id") FROM "ConfiguracoesClinica"), 1),
                true);
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
