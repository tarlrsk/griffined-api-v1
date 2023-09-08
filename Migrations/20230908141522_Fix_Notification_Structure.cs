using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FixNotificationStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffNotification_StudyCourse_StudyCourseId",
                table: "StaffNotification");

            migrationBuilder.RenameColumn(
                name: "hasRead",
                table: "TeacherNotification",
                newName: "HasRead");

            migrationBuilder.RenameColumn(
                name: "hasRead",
                table: "StudentNotification",
                newName: "HasRead");

            migrationBuilder.RenameColumn(
                name: "hasRead",
                table: "StaffNotification",
                newName: "HasRead");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "TeacherNotification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TeacherNotification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "StudentNotification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "StudentNotification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "StudyCourseId",
                table: "StaffNotification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CancellationRequestId",
                table: "StaffNotification",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "StaffNotification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RegistrationRequestId",
                table: "StaffNotification",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "StaffNotification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_CancellationRequestId",
                table: "StaffNotification",
                column: "CancellationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_RegistrationRequestId",
                table: "StaffNotification",
                column: "RegistrationRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffNotification_ClassCancellationRequest_CancellationRequestId",
                table: "StaffNotification",
                column: "CancellationRequestId",
                principalTable: "ClassCancellationRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffNotification_RegistrationRequest_RegistrationRequestId",
                table: "StaffNotification",
                column: "RegistrationRequestId",
                principalTable: "RegistrationRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffNotification_StudyCourse_StudyCourseId",
                table: "StaffNotification",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffNotification_ClassCancellationRequest_CancellationRequestId",
                table: "StaffNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffNotification_RegistrationRequest_RegistrationRequestId",
                table: "StaffNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffNotification_StudyCourse_StudyCourseId",
                table: "StaffNotification");

            migrationBuilder.DropIndex(
                name: "IX_StaffNotification_CancellationRequestId",
                table: "StaffNotification");

            migrationBuilder.DropIndex(
                name: "IX_StaffNotification_RegistrationRequestId",
                table: "StaffNotification");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "TeacherNotification");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TeacherNotification");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "StudentNotification");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "StudentNotification");

            migrationBuilder.DropColumn(
                name: "CancellationRequestId",
                table: "StaffNotification");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "StaffNotification");

            migrationBuilder.DropColumn(
                name: "RegistrationRequestId",
                table: "StaffNotification");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "StaffNotification");

            migrationBuilder.RenameColumn(
                name: "HasRead",
                table: "TeacherNotification",
                newName: "hasRead");

            migrationBuilder.RenameColumn(
                name: "HasRead",
                table: "StudentNotification",
                newName: "hasRead");

            migrationBuilder.RenameColumn(
                name: "HasRead",
                table: "StaffNotification",
                newName: "hasRead");

            migrationBuilder.AlterColumn<int>(
                name: "StudyCourseId",
                table: "StaffNotification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffNotification_StudyCourse_StudyCourseId",
                table: "StaffNotification",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
