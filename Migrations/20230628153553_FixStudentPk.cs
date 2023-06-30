using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FixStudentPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreferredDay_StudyCourse_studyCourseId",
                table: "PreferredDay");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffNotification_StudyCourse_studyCourseId",
                table: "StaffNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotification_StudyCourse_studyCourseId",
                table: "StudentNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCourseHistory_StudyCourse_studyCourseId",
                table: "StudyCourseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySubject_StudyCourse_studyCourseId",
                table: "StudySubject");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherNotification_StudyCourse_studyCourseId",
                table: "TeacherNotification");

            migrationBuilder.AddForeignKey(
                name: "FK_PreferredDay_StudyCourse_studyCourseId",
                table: "PreferredDay",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffNotification_StudyCourse_studyCourseId",
                table: "StaffNotification",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotification_StudyCourse_studyCourseId",
                table: "StudentNotification",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCourseHistory_StudyCourse_studyCourseId",
                table: "StudyCourseHistory",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySubject_StudyCourse_studyCourseId",
                table: "StudySubject",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherNotification_StudyCourse_studyCourseId",
                table: "TeacherNotification",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreferredDay_StudyCourse_studyCourseId",
                table: "PreferredDay");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffNotification_StudyCourse_studyCourseId",
                table: "StaffNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotification_StudyCourse_studyCourseId",
                table: "StudentNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCourseHistory_StudyCourse_studyCourseId",
                table: "StudyCourseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySubject_StudyCourse_studyCourseId",
                table: "StudySubject");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherNotification_StudyCourse_studyCourseId",
                table: "TeacherNotification");

            migrationBuilder.AddForeignKey(
                name: "FK_PreferredDay_StudyCourse_studyCourseId",
                table: "PreferredDay",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffNotification_StudyCourse_studyCourseId",
                table: "StaffNotification",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotification_StudyCourse_studyCourseId",
                table: "StudentNotification",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCourseHistory_StudyCourse_studyCourseId",
                table: "StudyCourseHistory",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySubject_StudyCourse_studyCourseId",
                table: "StudySubject",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherNotification_StudyCourse_studyCourseId",
                table: "TeacherNotification",
                column: "studyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
