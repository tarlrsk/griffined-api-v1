using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class TeacherCourseJoinedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudySubjectTeacher",
                columns: table => new
                {
                    StudySubjectTeacherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudySubjectMemberId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    CourseJoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySubjectTeacher", x => x.StudySubjectTeacherId);
                    table.ForeignKey(
                        name: "FK_StudySubjectTeacher_StudySubjectMember_StudySubjectMemberId",
                        column: x => x.StudySubjectMemberId,
                        principalTable: "StudySubjectMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudySubjectTeacher_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudySubjectTeacher_StudySubjectMemberId",
                table: "StudySubjectTeacher",
                column: "StudySubjectMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubjectTeacher_TeacherId",
                table: "StudySubjectTeacher",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudySubjectTeacher");
        }
    }
}
