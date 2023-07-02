using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FirebaseStorageAdditionalFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseRequest_Level_levelId",
                table: "NewCourseRequest");

            migrationBuilder.RenameColumn(
                name: "file",
                table: "StudentAdditionalFile",
                newName: "fileName");

            migrationBuilder.AlterColumn<int>(
                name: "levelId",
                table: "NewCourseRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseRequest_Level_levelId",
                table: "NewCourseRequest",
                column: "levelId",
                principalTable: "Level",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseRequest_Level_levelId",
                table: "NewCourseRequest");

            migrationBuilder.RenameColumn(
                name: "fileName",
                table: "StudentAdditionalFile",
                newName: "file");

            migrationBuilder.AlterColumn<int>(
                name: "levelId",
                table: "NewCourseRequest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseRequest_Level_levelId",
                table: "NewCourseRequest",
                column: "levelId",
                principalTable: "Level",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
