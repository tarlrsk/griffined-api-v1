using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class StudyCourseHistoryAddDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "StudyCourseHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "StudyCourseHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudyClassId",
                table: "StudyCourseHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourseHistory_StudentId",
                table: "StudyCourseHistory",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourseHistory_StudyClassId",
                table: "StudyCourseHistory",
                column: "StudyClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCourseHistory_Student_StudentId",
                table: "StudyCourseHistory",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCourseHistory_StudyClass_StudyClassId",
                table: "StudyCourseHistory",
                column: "StudyClassId",
                principalTable: "StudyClass",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyCourseHistory_Student_StudentId",
                table: "StudyCourseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCourseHistory_StudyClass_StudyClassId",
                table: "StudyCourseHistory");

            migrationBuilder.DropIndex(
                name: "IX_StudyCourseHistory_StudentId",
                table: "StudyCourseHistory");

            migrationBuilder.DropIndex(
                name: "IX_StudyCourseHistory_StudyClassId",
                table: "StudyCourseHistory");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "StudyCourseHistory");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudyCourseHistory");

            migrationBuilder.DropColumn(
                name: "StudyClassId",
                table: "StudyCourseHistory");
        }
    }
}
