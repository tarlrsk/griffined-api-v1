using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class CreateStudentAddingSubjectRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentAddingSubjectRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentAddingRequestId = table.Column<int>(type: "int", nullable: false),
                    StudySubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAddingSubjectRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAddingSubjectRequest_StudentAddingRequest_StudentAddingRequestId",
                        column: x => x.StudentAddingRequestId,
                        principalTable: "StudentAddingRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAddingSubjectRequest_StudySubject_StudySubjectId",
                        column: x => x.StudySubjectId,
                        principalTable: "StudySubject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingSubjectRequest_StudentAddingRequestId",
                table: "StudentAddingSubjectRequest",
                column: "StudentAddingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingSubjectRequest_StudySubjectId",
                table: "StudentAddingSubjectRequest",
                column: "StudySubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAddingSubjectRequest");
        }
    }
}
