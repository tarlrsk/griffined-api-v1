using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCourseMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentReport_CourseMember_CourseMemberId",
                table: "StudentReport");

            migrationBuilder.DropTable(
                name: "CourseMember");

            migrationBuilder.RenameColumn(
                name: "CourseMemberId",
                table: "StudentReport",
                newName: "StudySubjectMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentReport_CourseMemberId",
                table: "StudentReport",
                newName: "IX_StudentReport_StudySubjectMemberId");

            migrationBuilder.CreateTable(
                name: "StudySubjectMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudySubjectId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySubjectMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySubjectMember_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudySubjectMember_StudySubject_StudySubjectId",
                        column: x => x.StudySubjectId,
                        principalTable: "StudySubject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudySubjectMember_StudentId",
                table: "StudySubjectMember",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubjectMember_StudySubjectId",
                table: "StudySubjectMember",
                column: "StudySubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentReport_StudySubjectMember_StudySubjectMemberId",
                table: "StudentReport",
                column: "StudySubjectMemberId",
                principalTable: "StudySubjectMember",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentReport_StudySubjectMember_StudySubjectMemberId",
                table: "StudentReport");

            migrationBuilder.DropTable(
                name: "StudySubjectMember");

            migrationBuilder.RenameColumn(
                name: "StudySubjectMemberId",
                table: "StudentReport",
                newName: "CourseMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentReport_StudySubjectMemberId",
                table: "StudentReport",
                newName: "IX_StudentReport_CourseMemberId");

            migrationBuilder.CreateTable(
                name: "CourseMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudySubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMember_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseMember_StudySubject_StudySubjectId",
                        column: x => x.StudySubjectId,
                        principalTable: "StudySubject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseMember_StudentId",
                table: "CourseMember",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMember_StudySubjectId",
                table: "CourseMember",
                column: "StudySubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentReport_CourseMember_CourseMemberId",
                table: "StudentReport",
                column: "CourseMemberId",
                principalTable: "CourseMember",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
