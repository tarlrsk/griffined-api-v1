using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class RemodelClassCancellation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancellationRequest");

            migrationBuilder.CreateTable(
                name: "ClassCancellationRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false),
                    StudySubjectId = table.Column<int>(type: "int", nullable: false),
                    StudyClassId = table.Column<int>(type: "int", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassCancellationRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassCancellationRequest_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassCancellationRequest_StudyClass_StudyClassId",
                        column: x => x.StudyClassId,
                        principalTable: "StudyClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassCancellationRequest_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassCancellationRequest_StudySubject_StudySubjectId",
                        column: x => x.StudySubjectId,
                        principalTable: "StudySubject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassCancellationRequest_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassCancellationRequest_StudentId",
                table: "ClassCancellationRequest",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassCancellationRequest_StudyClassId",
                table: "ClassCancellationRequest",
                column: "StudyClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassCancellationRequest_StudyCourseId",
                table: "ClassCancellationRequest",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassCancellationRequest_StudySubjectId",
                table: "ClassCancellationRequest",
                column: "StudySubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassCancellationRequest_TeacherId",
                table: "ClassCancellationRequest",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassCancellationRequest");

            migrationBuilder.CreateTable(
                name: "CancellationRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudyClassId = table.Column<int>(type: "int", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_StudyClass_StudyClassId",
                        column: x => x.StudyClassId,
                        principalTable: "StudyClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_StudentId",
                table: "CancellationRequest",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_StudyClassId",
                table: "CancellationRequest",
                column: "StudyClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_StudyCourseId",
                table: "CancellationRequest",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_TeacherId",
                table: "CancellationRequest",
                column: "TeacherId");
        }
    }
}
