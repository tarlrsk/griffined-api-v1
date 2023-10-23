using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class MoveRoomToScheduleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "StudyClass");

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Schedule",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "Schedule");

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "StudyClass",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
