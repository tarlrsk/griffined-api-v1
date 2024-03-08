using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class AddMandays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTime_Teacher_TeacherId",
                table: "WorkTime");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "WorkTime");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "WorkTime",
                newName: "MandayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkTime_TeacherId",
                table: "WorkTime",
                newName: "IX_WorkTime_MandayId");

            migrationBuilder.CreateTable(
                name: "Mandays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mandays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mandays_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mandays_TeacherId",
                table: "Mandays",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTime_Mandays_MandayId",
                table: "WorkTime",
                column: "MandayId",
                principalTable: "Mandays",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTime_Mandays_MandayId",
                table: "WorkTime");

            migrationBuilder.DropTable(
                name: "Mandays");

            migrationBuilder.RenameColumn(
                name: "MandayId",
                table: "WorkTime",
                newName: "TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkTime_MandayId",
                table: "WorkTime",
                newName: "IX_WorkTime_TeacherId");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "WorkTime",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTime_Teacher_TeacherId",
                table: "WorkTime",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id");
        }
    }
}
