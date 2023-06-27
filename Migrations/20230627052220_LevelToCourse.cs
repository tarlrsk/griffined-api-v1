using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class LevelToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "courseId",
                table: "Level",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Level_courseId",
                table: "Level",
                column: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Level_Course_courseId",
                table: "Level",
                column: "courseId",
                principalTable: "Course",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Level_Course_courseId",
                table: "Level");

            migrationBuilder.DropIndex(
                name: "IX_Level_courseId",
                table: "Level");

            migrationBuilder.DropColumn(
                name: "courseId",
                table: "Level");
        }
    }
}
