using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FixRegistrationRequestStaffRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentByECId",
                table: "RegistrationRequest",
                newName: "ScheduledByStaffId");

            migrationBuilder.RenameColumn(
                name: "ByOAId",
                table: "RegistrationRequest",
                newName: "PaymentByStaffId");

            migrationBuilder.RenameColumn(
                name: "ByECId",
                table: "RegistrationRequest",
                newName: "CreatedByStaffId");

            migrationBuilder.RenameColumn(
                name: "ByEAId",
                table: "RegistrationRequest",
                newName: "ApprovedByStaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledByStaffId",
                table: "RegistrationRequest",
                newName: "PaymentByECId");

            migrationBuilder.RenameColumn(
                name: "PaymentByStaffId",
                table: "RegistrationRequest",
                newName: "ByOAId");

            migrationBuilder.RenameColumn(
                name: "CreatedByStaffId",
                table: "RegistrationRequest",
                newName: "ByECId");

            migrationBuilder.RenameColumn(
                name: "ApprovedByStaffId",
                table: "RegistrationRequest",
                newName: "ByEAId");
        }
    }
}
