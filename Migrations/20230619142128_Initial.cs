using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    appointmentType = table.Column<int>(type: "int", nullable: false),
                    appointmentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    paymentType = table.Column<int>(type: "int", nullable: false),
                    paymentStatus = table.Column<int>(type: "int", nullable: false),
                    byECId = table.Column<int>(type: "int", nullable: false),
                    byEAId = table.Column<int>(type: "int", nullable: false),
                    byOAId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationRequests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firebaseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firebaseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    profilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dob = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    line = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    school = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    countryOfSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    levelOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    program = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetUni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hogInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    healthInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firebaseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    appointmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.id);
                    table.ForeignKey(
                        name: "FK_Schedules_Appointments_appointmentId",
                        column: x => x.appointmentId,
                        principalTable: "Appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseId = table.Column<int>(type: "int", nullable: false),
                    subject = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.id);
                    table.ForeignKey(
                        name: "FK_Subjects_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewCourseRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false),
                    courseId = table.Column<int>(type: "int", nullable: false),
                    levelId = table.Column<int>(type: "int", nullable: false),
                    method = table.Column<int>(type: "int", nullable: false),
                    totalHours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewCourseRequests", x => x.id);
                    table.ForeignKey(
                        name: "FK_NewCourseRequests_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewCourseRequests_Levels_levelId",
                        column: x => x.levelId,
                        principalTable: "Levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewCourseRequests_RegistrationRequests_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentFile",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentFile", x => x.id);
                    table.ForeignKey(
                        name: "FK_PaymentFile_RegistrationRequests_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreferredDayRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferredDayRequests", x => x.id);
                    table.ForeignKey(
                        name: "FK_PreferredDayRequests_RegistrationRequests_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subdistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    district = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    zipcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.id);
                    table.ForeignKey(
                        name: "FK_Addresses_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.id);
                    table.ForeignKey(
                        name: "FK_Parents_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationRequestMembers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationRequestMembers", x => x.id);
                    table.ForeignKey(
                        name: "FK_RegistrationRequestMembers_RegistrationRequests_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrationRequestMembers_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAdditionalFiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAdditionalFiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentAdditionalFiles_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "AppointmentMembers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    appointmentId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentMembers", x => x.id);
                    table.ForeignKey(
                        name: "FK_AppointmentMembers_Appointments_appointmentId",
                        column: x => x.appointmentId,
                        principalTable: "Appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentMembers_Teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkTimes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTimes", x => x.id);
                    table.ForeignKey(
                        name: "FK_WorkTimes_Teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewCourseSubjectRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subjectId = table.Column<int>(type: "int", nullable: false),
                    newCourseRequestId = table.Column<int>(type: "int", nullable: false),
                    hour = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewCourseSubjectRequests", x => x.id);
                    table.ForeignKey(
                        name: "FK_NewCourseSubjectRequests_NewCourseRequests_newCourseRequestId",
                        column: x => x.newCourseRequestId,
                        principalTable: "NewCourseRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewCourseSubjectRequests_Subjects_subjectId",
                        column: x => x.subjectId,
                        principalTable: "Subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CancellationRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    requestedDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    classId = table.Column<int>(type: "int", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    studyClassId = table.Column<int>(type: "int", nullable: false),
                    StudySubjectid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationRequests", x => x.id);
                    table.ForeignKey(
                        name: "FK_CancellationRequests_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CancellationRequests_Teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMembers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studySubjectId = table.Column<int>(type: "int", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    Teacherid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMembers", x => x.id);
                    table.ForeignKey(
                        name: "FK_CourseMembers_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMembers_Teachers_Teacherid",
                        column: x => x.Teacherid,
                        principalTable: "Teachers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "StudentReports",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    report = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    progression = table.Column<int>(type: "int", nullable: false),
                    dateUploaded = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateUpdated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    courseMemberId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentReports", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentReports_CourseMembers_courseMemberId",
                        column: x => x.courseMemberId,
                        principalTable: "CourseMembers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentReports_Teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreferredDays",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferredDays", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StaffNotifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    hasRead = table.Column<bool>(type: "bit", nullable: false),
                    staffId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffNotifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_StaffNotifications_Staff_staffId",
                        column: x => x.staffId,
                        principalTable: "Staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyCourses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    StaffNotificationid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCourses", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudyCourses_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyCourses_StaffNotifications_StaffNotificationid",
                        column: x => x.StaffNotificationid,
                        principalTable: "StaffNotifications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "StudentAddingRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAddingRequests", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentAddingRequests_RegistrationRequests_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAddingRequests_StudyCourses_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentNotifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    hasRead = table.Column<bool>(type: "bit", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentNotifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentNotifications_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentNotifications_StudyCourses_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyCourseHistories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateUpdated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    byStaffId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCourseHistories", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudyCourseHistories_Staff_byStaffId",
                        column: x => x.byStaffId,
                        principalTable: "Staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyCourseHistories_StudyCourses_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudySubjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subjectId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySubjects", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudySubjects_StudyCourses_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudySubjects_Subjects_subjectId",
                        column: x => x.subjectId,
                        principalTable: "Subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherNotifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    hasRead = table.Column<bool>(type: "bit", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    appointmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherNotifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_TeacherNotifications_Appointments_appointmentId",
                        column: x => x.appointmentId,
                        principalTable: "Appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherNotifications_StudyCourses_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherNotifications_Teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyClasses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    classNumber = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    isMakeup = table.Column<bool>(type: "bit", nullable: false),
                    scheduleId = table.Column<int>(type: "int", nullable: false),
                    studySubjectId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyClasses", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudyClasses_Schedules_scheduleId",
                        column: x => x.scheduleId,
                        principalTable: "Schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyClasses_StudySubjects_studySubjectId",
                        column: x => x.studySubjectId,
                        principalTable: "StudySubjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyClasses_Teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAttendances",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    attendance = table.Column<int>(type: "int", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    attendanceId = table.Column<int>(type: "int", nullable: false),
                    studentAttendanceid = table.Column<int>(type: "int", nullable: false),
                    StudyClassid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttendances", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_StudentAttendances_studentAttendanceid",
                        column: x => x.studentAttendanceid,
                        principalTable: "StudentAttendances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_StudyClasses_StudyClassid",
                        column: x => x.StudyClassid,
                        principalTable: "StudyClasses",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_studentId",
                table: "Addresses",
                column: "studentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMembers_appointmentId",
                table: "AppointmentMembers",
                column: "appointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMembers_teacherId",
                table: "AppointmentMembers",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequests_studentId",
                table: "CancellationRequests",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequests_studyClassId",
                table: "CancellationRequests",
                column: "studyClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequests_studyCourseId",
                table: "CancellationRequests",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequests_StudySubjectid",
                table: "CancellationRequests",
                column: "StudySubjectid");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequests_teacherId",
                table: "CancellationRequests",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMembers_studentId",
                table: "CourseMembers",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMembers_studySubjectId",
                table: "CourseMembers",
                column: "studySubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMembers_Teacherid",
                table: "CourseMembers",
                column: "Teacherid");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequests_courseId",
                table: "NewCourseRequests",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequests_levelId",
                table: "NewCourseRequests",
                column: "levelId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequests_registrationRequestId",
                table: "NewCourseRequests",
                column: "registrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseSubjectRequests_newCourseRequestId",
                table: "NewCourseSubjectRequests",
                column: "newCourseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseSubjectRequests_subjectId",
                table: "NewCourseSubjectRequests",
                column: "subjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Parents_studentId",
                table: "Parents",
                column: "studentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentFile_registrationRequestId",
                table: "PaymentFile",
                column: "registrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferredDayRequests_registrationRequestId",
                table: "PreferredDayRequests",
                column: "registrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferredDays_studyCourseId",
                table: "PreferredDays",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestMembers_registrationRequestId",
                table: "RegistrationRequestMembers",
                column: "registrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestMembers_studentId",
                table: "RegistrationRequestMembers",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_appointmentId",
                table: "Schedules",
                column: "appointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotifications_staffId",
                table: "StaffNotifications",
                column: "staffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotifications_studyCourseId",
                table: "StaffNotifications",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingRequests_registrationRequestId",
                table: "StudentAddingRequests",
                column: "registrationRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingRequests_studyCourseId",
                table: "StudentAddingRequests",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdditionalFiles_studentId",
                table: "StudentAdditionalFiles",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_studentAttendanceid",
                table: "StudentAttendances",
                column: "studentAttendanceid");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_studentId",
                table: "StudentAttendances",
                column: "studentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudyClassid",
                table: "StudentAttendances",
                column: "StudyClassid");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotifications_studentId",
                table: "StudentNotifications",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotifications_studyCourseId",
                table: "StudentNotifications",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReports_courseMemberId",
                table: "StudentReports",
                column: "courseMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReports_teacherId",
                table: "StudentReports",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyClasses_scheduleId",
                table: "StudyClasses",
                column: "scheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyClasses_studySubjectId",
                table: "StudyClasses",
                column: "studySubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyClasses_teacherId",
                table: "StudyClasses",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourseHistories_byStaffId",
                table: "StudyCourseHistories",
                column: "byStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourseHistories_studyCourseId",
                table: "StudyCourseHistories",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourses_courseId",
                table: "StudyCourses",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourses_StaffNotificationid",
                table: "StudyCourses",
                column: "StaffNotificationid");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubjects_studyCourseId",
                table: "StudySubjects",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubjects_subjectId",
                table: "StudySubjects",
                column: "subjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_courseId",
                table: "Subjects",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotifications_appointmentId",
                table: "TeacherNotifications",
                column: "appointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotifications_studyCourseId",
                table: "TeacherNotifications",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotifications_teacherId",
                table: "TeacherNotifications",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimes_teacherId",
                table: "WorkTimes",
                column: "teacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancellationRequests_StudyClasses_studyClassId",
                table: "CancellationRequests",
                column: "studyClassId",
                principalTable: "StudyClasses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CancellationRequests_StudyCourses_studyCourseId",
                table: "CancellationRequests",
                column: "studyCourseId",
                principalTable: "StudyCourses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CancellationRequests_StudySubjects_StudySubjectid",
                table: "CancellationRequests",
                column: "StudySubjectid",
                principalTable: "StudySubjects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseMembers_StudySubjects_studySubjectId",
                table: "CourseMembers",
                column: "studySubjectId",
                principalTable: "StudySubjects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreferredDays_StudyCourses_studyCourseId",
                table: "PreferredDays",
                column: "studyCourseId",
                principalTable: "StudyCourses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffNotifications_StudyCourses_studyCourseId",
                table: "StaffNotifications",
                column: "studyCourseId",
                principalTable: "StudyCourses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffNotifications_StudyCourses_studyCourseId",
                table: "StaffNotifications");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "AppointmentMembers");

            migrationBuilder.DropTable(
                name: "CancellationRequests");

            migrationBuilder.DropTable(
                name: "NewCourseSubjectRequests");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropTable(
                name: "PaymentFile");

            migrationBuilder.DropTable(
                name: "PreferredDayRequests");

            migrationBuilder.DropTable(
                name: "PreferredDays");

            migrationBuilder.DropTable(
                name: "RegistrationRequestMembers");

            migrationBuilder.DropTable(
                name: "StudentAddingRequests");

            migrationBuilder.DropTable(
                name: "StudentAdditionalFiles");

            migrationBuilder.DropTable(
                name: "StudentAttendances");

            migrationBuilder.DropTable(
                name: "StudentNotifications");

            migrationBuilder.DropTable(
                name: "StudentReports");

            migrationBuilder.DropTable(
                name: "StudyCourseHistories");

            migrationBuilder.DropTable(
                name: "TeacherNotifications");

            migrationBuilder.DropTable(
                name: "WorkTimes");

            migrationBuilder.DropTable(
                name: "NewCourseRequests");

            migrationBuilder.DropTable(
                name: "StudyClasses");

            migrationBuilder.DropTable(
                name: "CourseMembers");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "RegistrationRequests");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "StudySubjects");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "StudyCourses");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "StaffNotifications");

            migrationBuilder.DropTable(
                name: "Staff");
        }
    }
}
