using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FixSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseSubjectRequests_Subjects_subjectId",
                table: "NewCourseSubjectRequests");

            migrationBuilder.AlterColumn<int>(
                name: "subjectId",
                table: "NewCourseSubjectRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseSubjectRequests_Subjects_subjectId",
                table: "NewCourseSubjectRequests",
                column: "subjectId",
                principalTable: "Subjects",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseSubjectRequests_Subjects_subjectId",
                table: "NewCourseSubjectRequests");

            migrationBuilder.AlterColumn<int>(
                name: "subjectId",
                table: "NewCourseSubjectRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseSubjectRequests_Subjects_subjectId",
                table: "NewCourseSubjectRequests",
                column: "subjectId",
                principalTable: "Subjects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
