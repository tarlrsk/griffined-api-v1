using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class RenamePreferredDayModelNoS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCoursePreferredDayRequests_RegistrationRequest_RegistrationRequestId",
                table: "NewCoursePreferredDayRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewCoursePreferredDayRequests",
                table: "NewCoursePreferredDayRequests");

            migrationBuilder.RenameTable(
                name: "NewCoursePreferredDayRequests",
                newName: "NewCoursePreferredDayRequest");

            migrationBuilder.RenameIndex(
                name: "IX_NewCoursePreferredDayRequests_RegistrationRequestId",
                table: "NewCoursePreferredDayRequest",
                newName: "IX_NewCoursePreferredDayRequest_RegistrationRequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewCoursePreferredDayRequest",
                table: "NewCoursePreferredDayRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewCoursePreferredDayRequest_RegistrationRequest_RegistrationRequestId",
                table: "NewCoursePreferredDayRequest",
                column: "RegistrationRequestId",
                principalTable: "RegistrationRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest",
                column: "NewCourseRequestId",
                principalTable: "NewCourseRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCoursePreferredDayRequest_RegistrationRequest_RegistrationRequestId",
                table: "NewCoursePreferredDayRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewCoursePreferredDayRequest",
                table: "NewCoursePreferredDayRequest");

            migrationBuilder.RenameTable(
                name: "NewCoursePreferredDayRequest",
                newName: "NewCoursePreferredDayRequests");

            migrationBuilder.RenameIndex(
                name: "IX_NewCoursePreferredDayRequest_RegistrationRequestId",
                table: "NewCoursePreferredDayRequests",
                newName: "IX_NewCoursePreferredDayRequests_RegistrationRequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewCoursePreferredDayRequests",
                table: "NewCoursePreferredDayRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewCoursePreferredDayRequests_RegistrationRequest_RegistrationRequestId",
                table: "NewCoursePreferredDayRequests",
                column: "RegistrationRequestId",
                principalTable: "RegistrationRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest",
                column: "NewCourseRequestId",
                principalTable: "NewCourseRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
