using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyStudentAddingRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseRequest_Level_levelId",
                table: "NewCourseRequest");

            migrationBuilder.DropIndex(
                name: "IX_StudentAddingRequest_registrationRequestId",
                table: "StudentAddingRequest");

            migrationBuilder.AlterColumn<int>(
                name: "levelId",
                table: "NewCourseRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingRequest_registrationRequestId",
                table: "StudentAddingRequest",
                column: "registrationRequestId");

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

            migrationBuilder.DropIndex(
                name: "IX_StudentAddingRequest_registrationRequestId",
                table: "StudentAddingRequest");

            migrationBuilder.AlterColumn<int>(
                name: "levelId",
                table: "NewCourseRequest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingRequest_registrationRequestId",
                table: "StudentAddingRequest",
                column: "registrationRequestId",
                unique: true);

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
