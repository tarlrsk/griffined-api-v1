using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppointmentType = table.Column<int>(type: "int", nullable: false),
                    AppointmentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ByECId = table.Column<int>(type: "int", nullable: true),
                    ByEAId = table.Column<int>(type: "int", nullable: true),
                    ByOAId = table.Column<int>(type: "int", nullable: true),
                    CancelledBy = table.Column<int>(type: "int", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentError = table.Column<bool>(type: "bit", nullable: false),
                    ScheduleError = table.Column<bool>(type: "bit", nullable: false),
                    NewCourseDetailError = table.Column<bool>(type: "bit", nullable: false),
                    HasSchedule = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationStatus = table.Column<int>(type: "int", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirebaseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirebaseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    School = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryOfSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LevelOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Program = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetUniversity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HogInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HealthInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teacher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirebaseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedule_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Level",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Level_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudyCourse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyCourse_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    subject = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subject_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: false),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_RegistrationRequest_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_RegistrationRequest_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subdistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Zipcode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Parent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parent_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationRequestMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationRequestMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationRequestMember_RegistrationRequest_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrationRequestMember_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentAdditionalFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAdditionalFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAdditionalFile_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentMember_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentMember_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkTime_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NewCourseRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: true),
                    TotalHours = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewCourseRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewCourseRequest_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewCourseRequest_Level_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Level",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NewCourseRequest_RegistrationRequest_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreferredDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferredDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreferredDay_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffNotification", x => x.Id);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentAddingRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAddingRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAddingRequest_RegistrationRequest_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAddingRequest_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudyCourseHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUpdated = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCourseHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyCourseHistory_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyCourseHistory_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    isRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherNotification_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherNotification_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherNotification_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudySubject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySubject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySubject_StudyCourse_StudyCourseId",
                        column: x => x.StudyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudySubject_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewCourseSubjectRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    NewCourseRequestId = table.Column<int>(type: "int", nullable: false),
                    Hour = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewCourseSubjectRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewCourseSubjectRequest_NewCourseRequest_NewCourseRequestId",
                        column: x => x.NewCourseRequestId,
                        principalTable: "NewCourseRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewCourseSubjectRequest_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "StudyClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    StudySubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    ClassNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    isMakeup = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyClass_Schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyClass_StudySubject_StudySubjectId",
                        column: x => x.StudySubjectId,
                        principalTable: "StudySubject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyClass_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentReport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseMemberId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Progression = table.Column<int>(type: "int", nullable: false),
                    DateUploaded = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUpdated = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentReport_CourseMember_CourseMemberId",
                        column: x => x.CourseMemberId,
                        principalTable: "CourseMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentReport_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CancellationRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    StudyCourseId = table.Column<int>(type: "int", nullable: false),
                    StudyClassId = table.Column<int>(type: "int", nullable: false),
                    RequestedDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "StudentAttendance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudyClassId = table.Column<int>(type: "int", nullable: false),
                    Attendance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttendance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAttendance_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAttendance_StudyClass_StudyClassId",
                        column: x => x.StudyClassId,
                        principalTable: "StudyClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_StudentId",
                table: "Address",
                column: "StudentId",
                unique: true,
                filter: "[StudentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMember_AppointmentId",
                table: "AppointmentMember",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMember_TeacherId",
                table: "AppointmentMember",
                column: "TeacherId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Comment_RegistrationRequestId",
                table: "Comment",
                column: "RegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_StaffId",
                table: "Comment",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMember_StudentId",
                table: "CourseMember",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMember_StudySubjectId",
                table: "CourseMember",
                column: "StudySubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Level_CourseId",
                table: "Level",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequest_CourseId",
                table: "NewCourseRequest",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequest_LevelId",
                table: "NewCourseRequest",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequest_RegistrationRequestId",
                table: "NewCourseRequest",
                column: "RegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseSubjectRequest_NewCourseRequestId",
                table: "NewCourseSubjectRequest",
                column: "NewCourseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseSubjectRequest_SubjectId",
                table: "NewCourseSubjectRequest",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Parent_StudentId",
                table: "Parent",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_RegistrationRequestId",
                table: "Payment",
                column: "RegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferredDay_StudyCourseId",
                table: "PreferredDay",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferredDayRequest_RegistrationRequestId",
                table: "PreferredDayRequest",
                column: "RegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestMember_RegistrationRequestId",
                table: "RegistrationRequestMember",
                column: "RegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestMember_StudentId",
                table: "RegistrationRequestMember",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_AppointmentId",
                table: "Schedule",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_StaffId",
                table: "StaffNotification",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_StudyCourseId",
                table: "StaffNotification",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingRequest_RegistrationRequestId",
                table: "StudentAddingRequest",
                column: "RegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingRequest_StudyCourseId",
                table: "StudentAddingRequest",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdditionalFile_StudentId",
                table: "StudentAdditionalFile",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendance_StudentId",
                table: "StudentAttendance",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendance_StudyClassId",
                table: "StudentAttendance",
                column: "StudyClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotification_StudentId",
                table: "StudentNotification",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotification_StudyCourseId",
                table: "StudentNotification",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReport_CourseMemberId",
                table: "StudentReport",
                column: "CourseMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReport_TeacherId",
                table: "StudentReport",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyClass_ScheduleId",
                table: "StudyClass",
                column: "ScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyClass_StudySubjectId",
                table: "StudyClass",
                column: "StudySubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyClass_TeacherId",
                table: "StudyClass",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourse_CourseId",
                table: "StudyCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourseHistory_StaffId",
                table: "StudyCourseHistory",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourseHistory_StudyCourseId",
                table: "StudyCourseHistory",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubject_StudyCourseId",
                table: "StudySubject",
                column: "StudyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubject_SubjectId",
                table: "StudySubject",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_CourseId",
                table: "Subject",
                column: "CourseId");

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

            migrationBuilder.CreateIndex(
                name: "IX_WorkTime_TeacherId",
                table: "WorkTime",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "AppointmentMember");

            migrationBuilder.DropTable(
                name: "CancellationRequest");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "NewCourseSubjectRequest");

            migrationBuilder.DropTable(
                name: "Parent");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "PreferredDay");

            migrationBuilder.DropTable(
                name: "PreferredDayRequest");

            migrationBuilder.DropTable(
                name: "RegistrationRequestMember");

            migrationBuilder.DropTable(
                name: "StaffNotification");

            migrationBuilder.DropTable(
                name: "StudentAddingRequest");

            migrationBuilder.DropTable(
                name: "StudentAdditionalFile");

            migrationBuilder.DropTable(
                name: "StudentAttendance");

            migrationBuilder.DropTable(
                name: "StudentNotification");

            migrationBuilder.DropTable(
                name: "StudentReport");

            migrationBuilder.DropTable(
                name: "StudyCourseHistory");

            migrationBuilder.DropTable(
                name: "TeacherNotification");

            migrationBuilder.DropTable(
                name: "WorkTime");

            migrationBuilder.DropTable(
                name: "NewCourseRequest");

            migrationBuilder.DropTable(
                name: "StudyClass");

            migrationBuilder.DropTable(
                name: "CourseMember");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Level");

            migrationBuilder.DropTable(
                name: "RegistrationRequest");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Teacher");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "StudySubject");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "StudyCourse");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "Course");
        }
    }
}
