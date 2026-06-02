using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendAI.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPainelTvProfissionalVinculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgendamentoId",
                table: "PainelTvChamadaAtual",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PacienteId",
                table: "PainelTvChamadaAtual",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfissionalId",
                table: "PainelTvChamadaAtual",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgendamentoId",
                table: "PainelTvChamadaAtual");

            migrationBuilder.DropColumn(
                name: "PacienteId",
                table: "PainelTvChamadaAtual");

            migrationBuilder.DropColumn(
                name: "ProfissionalId",
                table: "PainelTvChamadaAtual");
        }
    }
}
