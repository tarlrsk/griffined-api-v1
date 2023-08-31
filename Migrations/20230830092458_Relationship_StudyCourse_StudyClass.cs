using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipStudyCourseStudyClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudyCourseId",
                table: "StudyClass",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyClass_StudyCourseId",
                table: "StudyClass",
                column: "StudyCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass");

            migrationBuilder.DropIndex(
                name: "IX_StudyClass_StudyCourseId",
                table: "StudyClass");

            migrationBuilder.DropColumn(
                name: "StudyCourseId",
                table: "StudyClass");
        }
    }
}
