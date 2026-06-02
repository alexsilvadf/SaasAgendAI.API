using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendAI.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantScopedUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_TenantId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_Cpf",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_TenantId",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Atendimentos_Profissional_Data_Hora",
                table: "Atendimentos");

            migrationBuilder.DropIndex(
                name: "IX_Atendimentos_TenantId",
                table: "Atendimentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_Paciente_Data_Hora_Agendado",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_Profissional_Data_Hora_Agendado",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_TenantId",
                table: "Agendamentos");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                columns: new[] { "TenantId", "Email" },
                unique: true,
                filter: "\"Email\" IS NOT NULL AND \"Email\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios",
                columns: new[] { "TenantId", "Login" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_Cpf",
                table: "Pacientes",
                columns: new[] { "TenantId", "Cpf" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_Profissional_Data_Hora",
                table: "Atendimentos",
                columns: new[] { "TenantId", "ProfissionalId", "Data", "Hora" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_ProfissionalId",
                table: "Atendimentos",
                column: "ProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_Paciente_Data_Hora_Agendado",
                table: "Agendamentos",
                columns: new[] { "TenantId", "PacienteId", "Data", "HoraInicio" },
                unique: true,
                filter: "\"Status\" = 'agendado'");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_PacienteId",
                table: "Agendamentos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_Profissional_Data_Hora_Agendado",
                table: "Agendamentos",
                columns: new[] { "TenantId", "ProfissionalId", "Data", "HoraInicio" },
                unique: true,
                filter: "\"Status\" = 'agendado'");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ProfissionalId",
                table: "Agendamentos",
                column: "ProfissionalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_Cpf",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Atendimentos_Profissional_Data_Hora",
                table: "Atendimentos");

            migrationBuilder.DropIndex(
                name: "IX_Atendimentos_ProfissionalId",
                table: "Atendimentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_Paciente_Data_Hora_Agendado",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_PacienteId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_Profissional_Data_Hora_Agendado",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_ProfissionalId",
                table: "Agendamentos");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL AND \"Email\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TenantId",
                table: "Usuarios",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_Cpf",
                table: "Pacientes",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_TenantId",
                table: "Pacientes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_Profissional_Data_Hora",
                table: "Atendimentos",
                columns: new[] { "ProfissionalId", "Data", "Hora" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_TenantId",
                table: "Atendimentos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_Paciente_Data_Hora_Agendado",
                table: "Agendamentos",
                columns: new[] { "PacienteId", "Data", "HoraInicio" },
                unique: true,
                filter: "\"Status\" = 'agendado'");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_Profissional_Data_Hora_Agendado",
                table: "Agendamentos",
                columns: new[] { "ProfissionalId", "Data", "HoraInicio" },
                unique: true,
                filter: "\"Status\" = 'agendado'");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_TenantId",
                table: "Agendamentos",
                column: "TenantId");
        }
    }
}
