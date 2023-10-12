using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStudyCourseStaffNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySubject_StudyCourse_StudyCourseId",
                table: "StudySubject");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySubject_StudyCourse_StudyCourseId",
                table: "StudySubject",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySubject_StudyCourse_StudyCourseId",
                table: "StudySubject");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySubject_StudyCourse_StudyCourseId",
                table: "StudySubject",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
