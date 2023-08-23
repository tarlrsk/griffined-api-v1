using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class NullableRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isMakeup",
                table: "StudyClass",
                newName: "IsMakeup");

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "StudyClass",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "StudyClass");

            migrationBuilder.RenameColumn(
                name: "IsMakeup",
                table: "StudyClass",
                newName: "isMakeup");
        }
    }
}
