using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FixAppointmentHistoryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistory_Staff_StaffId",
                table: "AppointmentHistory");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AppointmentHistory");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "AppointmentHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentSlotId",
                table: "AppointmentHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "AppointmentHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AppointmentHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentHistory_AppointmentSlotId",
                table: "AppointmentHistory",
                column: "AppointmentSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistory_AppointmentSlot_AppointmentSlotId",
                table: "AppointmentHistory",
                column: "AppointmentSlotId",
                principalTable: "AppointmentSlot",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistory_Staff_StaffId",
                table: "AppointmentHistory",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistory_AppointmentSlot_AppointmentSlotId",
                table: "AppointmentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistory_Staff_StaffId",
                table: "AppointmentHistory");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentHistory_AppointmentSlotId",
                table: "AppointmentHistory");

            migrationBuilder.DropColumn(
                name: "AppointmentSlotId",
                table: "AppointmentHistory");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "AppointmentHistory");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppointmentHistory");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "AppointmentHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AppointmentHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistory_Staff_StaffId",
                table: "AppointmentHistory",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
