using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Appointment_AppointmentId",
                table: "Schedule");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_AppointmentId",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Schedule");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByStaffId",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppointmentSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    AppointmentSlotStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentSlot_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentSlot_Schedule_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_CreatedByStaffId",
                table: "Appointment",
                column: "CreatedByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlot_AppointmentId",
                table: "AppointmentSlot",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Staff_CreatedByStaffId",
                table: "Appointment",
                column: "CreatedByStaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Staff_CreatedByStaffId",
                table: "Appointment");

            migrationBuilder.DropTable(
                name: "AppointmentSlot");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_CreatedByStaffId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "CreatedByStaffId",
                table: "Appointment");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Schedule",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_AppointmentId",
                table: "Schedule",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Appointment_AppointmentId",
                table: "Schedule",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");
        }
    }
}
