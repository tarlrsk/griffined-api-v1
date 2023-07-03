using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class StudentAdditionalFileURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAdditionalFile_Student_studentId",
                table: "StudentAdditionalFile");

            migrationBuilder.RenameColumn(
                name: "studentId",
                table: "StudentAdditionalFile",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "fileName",
                table: "StudentAdditionalFile",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "StudentAdditionalFile",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAdditionalFile_studentId",
                table: "StudentAdditionalFile",
                newName: "IX_StudentAdditionalFile_StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAdditionalFile_Student_StudentId",
                table: "StudentAdditionalFile",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAdditionalFile_Student_StudentId",
                table: "StudentAdditionalFile");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "StudentAdditionalFile",
                newName: "studentId");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "StudentAdditionalFile",
                newName: "fileName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StudentAdditionalFile",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAdditionalFile_StudentId",
                table: "StudentAdditionalFile",
                newName: "IX_StudentAdditionalFile_studentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAdditionalFile_Student_studentId",
                table: "StudentAdditionalFile",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
