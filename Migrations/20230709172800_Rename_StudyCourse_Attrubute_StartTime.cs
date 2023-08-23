using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class RenameStudyCourseAttrubuteStartTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "StudyCourse",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "StudyCourse",
                newName: "EndDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "StudyCourse",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "StudyCourse",
                newName: "EndTime");
        }
    }
}
