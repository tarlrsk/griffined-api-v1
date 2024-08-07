using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudyCourseAndEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "StudyCourse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "StudyCourse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "StudyCourse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "StudyCourse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "StudyCourse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StudyCourseType",
                table: "StudyCourse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalHour",
                table: "StudyCourse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudyCourseType",
                table: "NewCourseRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourse_LevelId",
                table: "StudyCourse",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCourse_Level_LevelId",
                table: "StudyCourse",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyCourse_Level_LevelId",
                table: "StudyCourse");

            migrationBuilder.DropIndex(
                name: "IX_StudyCourse_LevelId",
                table: "StudyCourse");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "StudyCourse");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "StudyCourse");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "StudyCourse");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "StudyCourse");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "StudyCourse");

            migrationBuilder.DropColumn(
                name: "StudyCourseType",
                table: "StudyCourse");

            migrationBuilder.DropColumn(
                name: "TotalHour",
                table: "StudyCourse");

            migrationBuilder.DropColumn(
                name: "StudyCourseType",
                table: "NewCourseRequest");
        }
    }
}
