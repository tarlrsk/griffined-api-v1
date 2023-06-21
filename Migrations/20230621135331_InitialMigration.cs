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
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    appointmentType = table.Column<int>(type: "int", nullable: false),
                    appointmentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Level",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    byECId = table.Column<int>(type: "int", nullable: false),
                    byEAId = table.Column<int>(type: "int", nullable: false),
                    byOAId = table.Column<int>(type: "int", nullable: false),
                    section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    paymentType = table.Column<int>(type: "int", nullable: false),
                    paymentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationRequest", x => x.id);
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
                name: "Student",
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
                    table.PrimaryKey("PK_Student", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Teacher",
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
                    table.PrimaryKey("PK_Teacher", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    appointmentId = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.id);
                    table.ForeignKey(
                        name: "FK_Schedule_Appointment_appointmentId",
                        column: x => x.appointmentId,
                        principalTable: "Appointment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudyCourse",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCourse", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudyCourse_Course_courseId",
                        column: x => x.courseId,
                        principalTable: "Course",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseId = table.Column<int>(type: "int", nullable: false),
                    subject = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.id);
                    table.ForeignKey(
                        name: "FK_Subject_Course_courseId",
                        column: x => x.courseId,
                        principalTable: "Course",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewCourseRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false),
                    courseId = table.Column<int>(type: "int", nullable: false),
                    levelId = table.Column<int>(type: "int", nullable: false),
                    totalHours = table.Column<int>(type: "int", nullable: false),
                    method = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewCourseRequest", x => x.id);
                    table.ForeignKey(
                        name: "FK_NewCourseRequest_Course_courseId",
                        column: x => x.courseId,
                        principalTable: "Course",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewCourseRequest_Level_levelId",
                        column: x => x.levelId,
                        principalTable: "Level",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewCourseRequest_RegistrationRequest_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Payment_RegistrationRequest_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreferredDayRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false),
                    day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferredDayRequest", x => x.id);
                    table.ForeignKey(
                        name: "FK_PreferredDayRequest_RegistrationRequest_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subdistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    district = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    zipcode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.id);
                    table.ForeignKey(
                        name: "FK_Address_Student_studentId",
                        column: x => x.studentId,
                        principalTable: "Student",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Parent",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    fName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    line = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parent", x => x.id);
                    table.ForeignKey(
                        name: "FK_Parent_Student_studentId",
                        column: x => x.studentId,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationRequestMember",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationRequestMember", x => x.id);
                    table.ForeignKey(
                        name: "FK_RegistrationRequestMember_RegistrationRequest_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrationRequestMember_Student_studentId",
                        column: x => x.studentId,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentAdditionalFile",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAdditionalFile", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentAdditionalFile_Student_studentId",
                        column: x => x.studentId,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentMember",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    appointmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentMember", x => x.id);
                    table.ForeignKey(
                        name: "FK_AppointmentMember_Appointment_appointmentId",
                        column: x => x.appointmentId,
                        principalTable: "Appointment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentMember_Teacher_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teacher",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkTime",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacherId = table.Column<int>(type: "int", nullable: true),
                    day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTime", x => x.id);
                    table.ForeignKey(
                        name: "FK_WorkTime_Teacher_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teacher",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PreferredDay",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferredDay", x => x.id);
                    table.ForeignKey(
                        name: "FK_PreferredDay_StudyCourse_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffNotification",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    staffId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    hasRead = table.Column<bool>(type: "bit", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffNotification", x => x.id);
                    table.ForeignKey(
                        name: "FK_StaffNotification_Staff_staffId",
                        column: x => x.staffId,
                        principalTable: "Staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffNotification_StudyCourse_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAddingRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    registrationRequestId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAddingRequest", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentAddingRequest_RegistrationRequest_registrationRequestId",
                        column: x => x.registrationRequestId,
                        principalTable: "RegistrationRequest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAddingRequest_StudyCourse_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentNotification",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    hasRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentNotification", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentNotification_Student_studentId",
                        column: x => x.studentId,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentNotification_StudyCourse_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyCourseHistory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    staffId = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateUpdated = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCourseHistory", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudyCourseHistory_Staff_staffId",
                        column: x => x.staffId,
                        principalTable: "Staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyCourseHistory_StudyCourse_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherNotification",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    appointmentId = table.Column<int>(type: "int", nullable: false),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    hasRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherNotification", x => x.id);
                    table.ForeignKey(
                        name: "FK_TeacherNotification_Appointment_appointmentId",
                        column: x => x.appointmentId,
                        principalTable: "Appointment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherNotification_StudyCourse_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherNotification_Teacher_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teacher",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudySubject",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subjectId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySubject", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudySubject_StudyCourse_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudySubject_Subject_subjectId",
                        column: x => x.subjectId,
                        principalTable: "Subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewCourseSubjectRequest",
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
                    table.PrimaryKey("PK_NewCourseSubjectRequest", x => x.id);
                    table.ForeignKey(
                        name: "FK_NewCourseSubjectRequest_NewCourseRequest_newCourseRequestId",
                        column: x => x.newCourseRequestId,
                        principalTable: "NewCourseRequest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewCourseSubjectRequest_Subject_subjectId",
                        column: x => x.subjectId,
                        principalTable: "Subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseMember",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    studySubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMember", x => x.id);
                    table.ForeignKey(
                        name: "FK_CourseMember_Student_studentId",
                        column: x => x.studentId,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseMember_StudySubject_studySubjectId",
                        column: x => x.studySubjectId,
                        principalTable: "StudySubject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudyClass",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    classNumber = table.Column<int>(type: "int", nullable: false),
                    scheduleId = table.Column<int>(type: "int", nullable: false),
                    studySubjectId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    isMakeup = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyClass", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudyClass_Schedule_scheduleId",
                        column: x => x.scheduleId,
                        principalTable: "Schedule",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyClass_StudySubject_studySubjectId",
                        column: x => x.studySubjectId,
                        principalTable: "StudySubject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyClass_Teacher_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teacher",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentReport",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseMemberId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    report = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    progression = table.Column<int>(type: "int", nullable: false),
                    dateUploaded = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateUpdated = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentReport", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentReport_CourseMember_courseMemberId",
                        column: x => x.courseMemberId,
                        principalTable: "CourseMember",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentReport_Teacher_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teacher",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CancellationRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    classId = table.Column<int>(type: "int", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    studyCourseId = table.Column<int>(type: "int", nullable: false),
                    studyClassId = table.Column<int>(type: "int", nullable: false),
                    requestedDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationRequest", x => x.id);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_Student_studentId",
                        column: x => x.studentId,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_StudyClass_studyClassId",
                        column: x => x.studyClassId,
                        principalTable: "StudyClass",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_StudyCourse_studyCourseId",
                        column: x => x.studyCourseId,
                        principalTable: "StudyCourse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancellationRequest_Teacher_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teacher",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentAttendance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    studyClassId = table.Column<int>(type: "int", nullable: false),
                    attendance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttendance", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentAttendance_Student_studentId",
                        column: x => x.studentId,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAttendance_StudyClass_studyClassId",
                        column: x => x.studyClassId,
                        principalTable: "StudyClass",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_studentId",
                table: "Address",
                column: "studentId",
                unique: true,
                filter: "[studentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMember_appointmentId",
                table: "AppointmentMember",
                column: "appointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMember_teacherId",
                table: "AppointmentMember",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_studentId",
                table: "CancellationRequest",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_studyClassId",
                table: "CancellationRequest",
                column: "studyClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_studyCourseId",
                table: "CancellationRequest",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRequest_teacherId",
                table: "CancellationRequest",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMember_studentId",
                table: "CourseMember",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMember_studySubjectId",
                table: "CourseMember",
                column: "studySubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequest_courseId",
                table: "NewCourseRequest",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequest_levelId",
                table: "NewCourseRequest",
                column: "levelId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseRequest_registrationRequestId",
                table: "NewCourseRequest",
                column: "registrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseSubjectRequest_newCourseRequestId",
                table: "NewCourseSubjectRequest",
                column: "newCourseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NewCourseSubjectRequest_subjectId",
                table: "NewCourseSubjectRequest",
                column: "subjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Parent_studentId",
                table: "Parent",
                column: "studentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_registrationRequestId",
                table: "Payment",
                column: "registrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferredDay_studyCourseId",
                table: "PreferredDay",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferredDayRequest_registrationRequestId",
                table: "PreferredDayRequest",
                column: "registrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestMember_registrationRequestId",
                table: "RegistrationRequestMember",
                column: "registrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestMember_studentId",
                table: "RegistrationRequestMember",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_appointmentId",
                table: "Schedule",
                column: "appointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_staffId",
                table: "StaffNotification",
                column: "staffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffNotification_studyCourseId",
                table: "StaffNotification",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingRequest_registrationRequestId",
                table: "StudentAddingRequest",
                column: "registrationRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAddingRequest_studyCourseId",
                table: "StudentAddingRequest",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdditionalFile_studentId",
                table: "StudentAdditionalFile",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendance_studentId",
                table: "StudentAttendance",
                column: "studentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendance_studyClassId",
                table: "StudentAttendance",
                column: "studyClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotification_studentId",
                table: "StudentNotification",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotification_studyCourseId",
                table: "StudentNotification",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReport_courseMemberId",
                table: "StudentReport",
                column: "courseMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReport_teacherId",
                table: "StudentReport",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyClass_scheduleId",
                table: "StudyClass",
                column: "scheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyClass_studySubjectId",
                table: "StudyClass",
                column: "studySubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyClass_teacherId",
                table: "StudyClass",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourse_courseId",
                table: "StudyCourse",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourseHistory_staffId",
                table: "StudyCourseHistory",
                column: "staffId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCourseHistory_studyCourseId",
                table: "StudyCourseHistory",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubject_studyCourseId",
                table: "StudySubject",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySubject_subjectId",
                table: "StudySubject",
                column: "subjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_courseId",
                table: "Subject",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotification_appointmentId",
                table: "TeacherNotification",
                column: "appointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotification_studyCourseId",
                table: "TeacherNotification",
                column: "studyCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherNotification_teacherId",
                table: "TeacherNotification",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTime_teacherId",
                table: "WorkTime",
                column: "teacherId");
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
