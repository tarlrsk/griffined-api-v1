using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameObjectNameStudentReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Report",
                table: "StudentReport",
                newName: "ObjectName");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "StudentReport",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "StudentReport");

            migrationBuilder.RenameColumn(
                name: "ObjectName",
                table: "StudentReport",
                newName: "Report");
        }
    }
}
