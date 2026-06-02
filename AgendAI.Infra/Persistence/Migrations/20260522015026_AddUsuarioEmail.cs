using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendAI.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL AND \"Email\" <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Usuarios");
        }
    }
}
