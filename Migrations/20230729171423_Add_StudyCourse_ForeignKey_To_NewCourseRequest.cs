using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyCourseForeignKeyToNewCourseRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudyCourseId",
                table: "NewCourseRequest",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequest_StudyCourseId",
                table: "NewCourseRequest",
                column: "StudyCourseId",
                unique: true,
                filter: "[StudyCourseId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseRequest_StudyCourse_StudyCourseId",
                table: "NewCourseRequest",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseRequest_StudyCourse_StudyCourseId",
                table: "NewCourseRequest");

            migrationBuilder.DropIndex(
                name: "IX_NewCourseRequest_StudyCourseId",
                table: "NewCourseRequest");

            migrationBuilder.DropColumn(
                name: "StudyCourseId",
                table: "NewCourseRequest");
        }
    }
}
