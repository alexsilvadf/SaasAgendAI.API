using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendAI.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConfiguracoesClinica",
                keyColumn: "Id",
                keyValue: 1,
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TenantId",
                table: "Usuarios",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TokensRecuperacaoSenha_TenantId",
                table: "TokensRecuperacaoSenha",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedimentos_TenantId",
                table: "Procedimentos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PainelTvChamadaAtual_TenantId",
                table: "PainelTvChamadaAtual",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_TenantId",
                table: "Pacientes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PacienteHistoricos_TenantId",
                table: "PacienteHistoricos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PacienteAnamneses_TenantId",
                table: "PacienteAnamneses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_TenantId",
                table: "Lancamentos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesClinica_TenantId",
                table: "ConfiguracoesClinica",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueiosAgenda_TenantId",
                table: "BloqueiosAgenda",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_TenantId",
                table: "Atendimentos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_TenantId",
                table: "Agendamentos",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Tenants_TenantId",
                table: "Agendamentos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Atendimentos_Tenants_TenantId",
                table: "Atendimentos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BloqueiosAgenda_Tenants_TenantId",
                table: "BloqueiosAgenda",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracoesClinica_Tenants_TenantId",
                table: "ConfiguracoesClinica",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_Tenants_TenantId",
                table: "Lancamentos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PacienteAnamneses_Tenants_TenantId",
                table: "PacienteAnamneses",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PacienteHistoricos_Tenants_TenantId",
                table: "PacienteHistoricos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pacientes_Tenants_TenantId",
                table: "Pacientes",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PainelTvChamadaAtual_Tenants_TenantId",
                table: "PainelTvChamadaAtual",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Procedimentos_Tenants_TenantId",
                table: "Procedimentos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TokensRecuperacaoSenha_Tenants_TenantId",
                table: "TokensRecuperacaoSenha",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Tenants_TenantId",
                table: "Usuarios",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Tenants_TenantId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Atendimentos_Tenants_TenantId",
                table: "Atendimentos");

            migrationBuilder.DropForeignKey(
                name: "FK_BloqueiosAgenda_Tenants_TenantId",
                table: "BloqueiosAgenda");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracoesClinica_Tenants_TenantId",
                table: "ConfiguracoesClinica");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_Tenants_TenantId",
                table: "Lancamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_PacienteAnamneses_Tenants_TenantId",
                table: "PacienteAnamneses");

            migrationBuilder.DropForeignKey(
                name: "FK_PacienteHistoricos_Tenants_TenantId",
                table: "PacienteHistoricos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pacientes_Tenants_TenantId",
                table: "Pacientes");

            migrationBuilder.DropForeignKey(
                name: "FK_PainelTvChamadaAtual_Tenants_TenantId",
                table: "PainelTvChamadaAtual");

            migrationBuilder.DropForeignKey(
                name: "FK_Procedimentos_Tenants_TenantId",
                table: "Procedimentos");

            migrationBuilder.DropForeignKey(
                name: "FK_TokensRecuperacaoSenha_Tenants_TenantId",
                table: "TokensRecuperacaoSenha");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Tenants_TenantId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_TenantId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_TokensRecuperacaoSenha_TenantId",
                table: "TokensRecuperacaoSenha");

            migrationBuilder.DropIndex(
                name: "IX_Procedimentos_TenantId",
                table: "Procedimentos");

            migrationBuilder.DropIndex(
                name: "IX_PainelTvChamadaAtual_TenantId",
                table: "PainelTvChamadaAtual");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_TenantId",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_PacienteHistoricos_TenantId",
                table: "PacienteHistoricos");

            migrationBuilder.DropIndex(
                name: "IX_PacienteAnamneses_TenantId",
                table: "PacienteAnamneses");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_TenantId",
                table: "Lancamentos");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracoesClinica_TenantId",
                table: "ConfiguracoesClinica");

            migrationBuilder.DropIndex(
                name: "IX_BloqueiosAgenda_TenantId",
                table: "BloqueiosAgenda");

            migrationBuilder.DropIndex(
                name: "IX_Atendimentos_TenantId",
                table: "Atendimentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_TenantId",
                table: "Agendamentos");

            migrationBuilder.UpdateData(
                table: "ConfiguracoesClinica",
                keyColumn: "Id",
                keyValue: 1,
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000001"));
        }
    }
}
