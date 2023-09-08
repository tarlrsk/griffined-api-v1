using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FixNotificationsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotification_StudyCourse_StudyCourseId",
                table: "StudentNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherNotification_Appointment_AppointmentId",
                table: "TeacherNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherNotification_StudyCourse_StudyCourseId",
                table: "TeacherNotification");

            migrationBuilder.AlterColumn<int>(
                name: "StudyCourseId",
                table: "TeacherNotification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "TeacherNotification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StudyCourseId",
                table: "StudentNotification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotification_StudyCourse_StudyCourseId",
                table: "StudentNotification",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherNotification_Appointment_AppointmentId",
                table: "TeacherNotification",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherNotification_StudyCourse_StudyCourseId",
                table: "TeacherNotification",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotification_StudyCourse_StudyCourseId",
                table: "StudentNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherNotification_Appointment_AppointmentId",
                table: "TeacherNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherNotification_StudyCourse_StudyCourseId",
                table: "TeacherNotification");

            migrationBuilder.AlterColumn<int>(
                name: "StudyCourseId",
                table: "TeacherNotification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "TeacherNotification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudyCourseId",
                table: "StudentNotification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotification_StudyCourse_StudyCourseId",
                table: "StudentNotification",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherNotification_Appointment_AppointmentId",
                table: "TeacherNotification",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherNotification_StudyCourse_StudyCourseId",
                table: "TeacherNotification",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
