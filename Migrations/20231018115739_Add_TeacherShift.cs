using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeacherShift",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    StudyClassId = table.Column<int>(type: "int", nullable: true),
                    TeacherWorkType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherShift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherShift_StudyClass_StudyClassId",
                        column: x => x.StudyClassId,
                        principalTable: "StudyClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherShift_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherShift_StudyClassId",
                table: "TeacherShift",
                column: "StudyClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherShift_TeacherId",
                table: "TeacherShift",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherShift");
        }
    }
}
