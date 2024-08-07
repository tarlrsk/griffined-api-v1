using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySubject_StudyCourse_StudyCourseId",
                table: "StudySubject");

            migrationBuilder.DropTable(
                name: "StaffNotification");

            migrationBuilder.DropTable(
                name: "StudentNotification");

            migrationBuilder.DropTable(
                name: "TeacherNotification");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySubject_StudyCourse_StudyCourseId",
                table: "StudySubject",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySubject_StudyCourse_StudyCourseId",
                table: "StudySubject");

            migrationBuilder.CreateTable(
                name: "StaffNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CancellationRequestId = table.Column<int>(type: "int", nullable: true),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: true),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasRead = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffNotification_ClassCancellationRequest_CancellationRequestId",
                        column: x => x.CancellationRequestId,
                        principalTable: "ClassCancellationRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffNotification_RegistrationRequest_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffNotification_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffNotification_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasRead = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentNotification_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentNotification_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TeacherNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: true),
                    StudyCourseId = table.Column<int>(type: "int", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasRead = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherNotification_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherNotification_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherNotification_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_CancellationRequestId",
                table: "StaffNotification",
                column: "CancellationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_RegistrationRequestId",
                table: "StaffNotification",
                column: "RegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_StaffId",
                table: "StaffNotification",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_StudyCourseId",
                table: "StaffNotification",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotification_StudentId",
                table: "StudentNotification",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotification_StudyCourseId",
                table: "StudentNotification",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotification_AppointmentId",
                table: "TeacherNotification",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotification_StudyCourseId",
                table: "TeacherNotification",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotification_TeacherId",
                table: "TeacherNotification",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyClass_StudyCourse_StudyCourseId",
                table: "StudyClass",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySubject_StudyCourse_StudyCourseId",
                table: "StudySubject",
                column: "StudyCourseId",
                principalTable: "StudyCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
