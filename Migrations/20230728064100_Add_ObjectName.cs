using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "URL",
                table: "StudentAdditionalFile",
                newName: "ObjectName");

            migrationBuilder.RenameColumn(
                name: "URL",
                table: "ProfilePicture",
                newName: "ObjectName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ObjectName",
                table: "StudentAdditionalFile",
                newName: "URL");

            migrationBuilder.RenameColumn(
                name: "ObjectName",
                table: "ProfilePicture",
                newName: "URL");
        }
    }
}
