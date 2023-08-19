using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class StudentAttendanceNullableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendance_Student_StudentId",
                table: "StudentAttendance");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendance_StudyClass_StudyClassId",
                table: "StudentAttendance");

            migrationBuilder.DropIndex(
                name: "IX_StudentAttendance_StudentId",
                table: "StudentAttendance");

            migrationBuilder.AlterColumn<int>(
                name: "StudyClassId",
                table: "StudentAttendance",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentAttendance",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendance_StudentId",
                table: "StudentAttendance",
                column: "StudentId",
                unique: true,
                filter: "[StudentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendance_Student_StudentId",
                table: "StudentAttendance",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendance_StudyClass_StudyClassId",
                table: "StudentAttendance",
                column: "StudyClassId",
                principalTable: "StudyClass",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendance_Student_StudentId",
                table: "StudentAttendance");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendance_StudyClass_StudyClassId",
                table: "StudentAttendance");

            migrationBuilder.DropIndex(
                name: "IX_StudentAttendance_StudentId",
                table: "StudentAttendance");

            migrationBuilder.AlterColumn<int>(
                name: "StudyClassId",
                table: "StudentAttendance",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentAttendance",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendance_StudentId",
                table: "StudentAttendance",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendance_Student_StudentId",
                table: "StudentAttendance",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendance_StudyClass_StudyClassId",
                table: "StudentAttendance",
                column: "StudyClassId",
                principalTable: "StudyClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
