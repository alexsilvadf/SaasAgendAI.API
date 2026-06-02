using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AgendAI.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantsTableAndDefaultSeed : Migration
    {
        private static readonly Guid TenantDefaultId = new("00000000-0000-0000-0000-000000000001");
        private static readonly DateTime TenantDefaultCriadoEm =
            new(2026, 6, 2, 0, 0, 0, DateTimeKind.Utc);

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Usuarios",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "TokensRecuperacaoSenha",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Procedimentos",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PainelTvChamadaAtual",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "PainelTvChamadaAtual",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Pacientes",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "PacienteHistoricos",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "PacienteAnamneses",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Lancamentos",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ConfiguracoesClinica",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "BloqueiosAgenda",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Atendimentos",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Agendamentos",
                type: "uuid",
                nullable: false,
                defaultValue: TenantDefaultId);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Ativo", "CriadoEm", "Nome", "Slug" },
                values: new object[] { TenantDefaultId, true, TenantDefaultCriadoEm, "Clinica Padrao", "default" });

            migrationBuilder.UpdateData(
                table: "ConfiguracoesClinica",
                keyColumn: "Id",
                keyValue: 1,
                column: "TenantId",
                value: TenantDefaultId);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TokensRecuperacaoSenha");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Procedimentos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PainelTvChamadaAtual");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PacienteHistoricos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PacienteAnamneses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ConfiguracoesClinica");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BloqueiosAgenda");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Atendimentos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Agendamentos");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PainelTvChamadaAtual",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
