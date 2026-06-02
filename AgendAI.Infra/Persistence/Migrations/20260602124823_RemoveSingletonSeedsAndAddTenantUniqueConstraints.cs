using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendAI.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSingletonSeedsAndAddTenantUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PainelTvChamadaAtual_TenantId",
                table: "PainelTvChamadaAtual");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracoesClinica_TenantId",
                table: "ConfiguracoesClinica");

            migrationBuilder.DeleteData(
                table: "ConfiguracoesClinica",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_PainelTvChamadaAtual_TenantId",
                table: "PainelTvChamadaAtual",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesClinica_TenantId",
                table: "ConfiguracoesClinica",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PainelTvChamadaAtual_TenantId",
                table: "PainelTvChamadaAtual");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracoesClinica_TenantId",
                table: "ConfiguracoesClinica");

            migrationBuilder.InsertData(
                table: "ConfiguracoesClinica",
                columns: new[] { "Id", "HoraAbertura", "HoraFechamento", "IntervaloMinutos", "TenantId" },
                values: new object[] { 1, new TimeOnly(8, 0, 0), new TimeOnly(18, 0, 0), 30, new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.CreateIndex(
                name: "IX_PainelTvChamadaAtual_TenantId",
                table: "PainelTvChamadaAtual",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesClinica_TenantId",
                table: "ConfiguracoesClinica",
                column: "TenantId");
        }
    }
}
