using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendAI.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPainelTvChamadaAtual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PainelTvChamadaAtual",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    PacienteNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProfissionalNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Horario = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PainelTvChamadaAtual", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PainelTvChamadaAtual");
        }
    }
}
