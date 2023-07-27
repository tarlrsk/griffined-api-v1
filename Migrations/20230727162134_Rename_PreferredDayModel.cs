using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class RenamePreferredDayModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest");

            migrationBuilder.DropTable(
                name: "PreferredDayRequest");

            migrationBuilder.CreateTable(
                name: "NewCoursePreferredDayRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewCoursePreferredDayRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewCoursePreferredDayRequests_RegistrationRequest_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewCoursePreferredDayRequests_RegistrationRequestId",
                table: "NewCoursePreferredDayRequests",
                column: "RegistrationRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest",
                column: "NewCourseRequestId",
                principalTable: "NewCourseRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest");

            migrationBuilder.DropTable(
                name: "NewCoursePreferredDayRequests");

            migrationBuilder.CreateTable(
                name: "PreferredDayRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferredDayRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreferredDayRequest_RegistrationRequest_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreferredDayRequest_RegistrationRequestId",
                table: "PreferredDayRequest",
                column: "RegistrationRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest",
                column: "NewCourseRequestId",
                principalTable: "NewCourseRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
