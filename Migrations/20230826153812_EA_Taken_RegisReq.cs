using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class EATakenRegisReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TakenByEAId",
                table: "RegistrationRequest",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TakenByEAId",
                table: "RegistrationRequest");
        }
    }
}
