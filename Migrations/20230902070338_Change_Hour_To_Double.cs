using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeHourToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass");

            migrationBuilder.AlterColumn<double>(
                name: "Hour",
                table: "StudySubject",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "TotalHour",
                table: "StudyCourse",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StudyCourseId",
                table: "StudyClass",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Hour",
                table: "NewCourseSubjectRequest",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "TotalHours",
                table: "NewCourseRequest",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass");

            migrationBuilder.AlterColumn<int>(
                name: "Hour",
                table: "StudySubject",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "TotalHour",
                table: "StudyCourse",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "StudyCourseId",
                table: "StudyClass",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Hour",
                table: "NewCourseSubjectRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "TotalHours",
                table: "NewCourseRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id");
        }
    }
}
