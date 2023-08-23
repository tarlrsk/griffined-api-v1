using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FixStudentNullableProfilePic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfilePicture_Student_StudentId",
                table: "ProfilePicture");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAdditionalFile_Student_StudentId",
                table: "StudentAdditionalFile");

            migrationBuilder.DropIndex(
                name: "IX_ProfilePicture_StudentId",
                table: "ProfilePicture");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentAdditionalFile",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "ProfilePicture",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePicture_StudentId",
                table: "ProfilePicture",
                column: "StudentId",
                unique: true,
                filter: "[StudentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfilePicture_Student_StudentId",
                table: "ProfilePicture",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAdditionalFile_Student_StudentId",
                table: "StudentAdditionalFile",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfilePicture_Student_StudentId",
                table: "ProfilePicture");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAdditionalFile_Student_StudentId",
                table: "StudentAdditionalFile");

            migrationBuilder.DropIndex(
                name: "IX_ProfilePicture_StudentId",
                table: "ProfilePicture");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentAdditionalFile",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "ProfilePicture",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePicture_StudentId",
                table: "ProfilePicture",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfilePicture_Student_StudentId",
                table: "ProfilePicture",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAdditionalFile_Student_StudentId",
                table: "StudentAdditionalFile",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
