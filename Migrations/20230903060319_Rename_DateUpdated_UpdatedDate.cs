using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateUpdatedUpdatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateUpdated",
                table: "StudyCourseHistory",
                newName: "UpdatedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "StudyCourseHistory",
                newName: "DateUpdated");
        }
    }
}
