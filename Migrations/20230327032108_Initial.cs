using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace houseofgriffinapi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    level = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "EAs",
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
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "EPs",
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
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EPs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OAs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firebaseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
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
                    table.PrimaryKey("PK_Staffs", x => x.id);
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
                    line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    school = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    countryOfSchool = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    levelOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    program = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    targetUni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hogInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    healthInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
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
                name: "PrivateRegistrationRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    courseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    EAStatus = table.Column<int>(type: "int", nullable: false),
                    paymentStatus = table.Column<int>(type: "int", nullable: false),
                    takenByEPId = table.Column<int>(type: "int", nullable: true),
                    takenByEAId = table.Column<int>(type: "int", nullable: true),
                    takenByOAId = table.Column<int>(type: "int", nullable: true),
                    EPRemark1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EPRemark2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EARemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OARemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true),
                    Staffid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateRegistrationRequests", x => x.id);
                    table.ForeignKey(
                        name: "FK_PrivateRegistrationRequests_Staffs_Staffid",
                        column: x => x.Staffid,
                        principalTable: "Staffs",
                        principalColumn: "id");
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
                name: "TeacherLeavingRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    fromDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    teacherRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EARemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OARemark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherLeavingRequests", x => x.id);
                    table.ForeignKey(
                        name: "FK_TeacherLeavingRequests_Teachers_teacherId",
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
                name: "Payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paymentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    privateRegReqId = table.Column<int>(type: "int", nullable: false),
                    groupRegReqId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Payments_PrivateRegistrationRequests_privateRegReqId",
                        column: x => x.privateRegReqId,
                        principalTable: "PrivateRegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateCourses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    method = table.Column<int>(type: "int", nullable: false),
                    totalHour = table.Column<int>(type: "int", nullable: false),
                    hourPerClass = table.Column<int>(type: "int", nullable: false),
                    fromDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reqId = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true),
                    requestid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateCourses", x => x.id);
                    table.ForeignKey(
                        name: "FK_PrivateCourses_PrivateRegistrationRequests_requestid",
                        column: x => x.requestid,
                        principalTable: "PrivateRegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateRegistrationRequestInfos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    totalHour = table.Column<int>(type: "int", nullable: false),
                    method = table.Column<int>(type: "int", nullable: false),
                    hourPerClass = table.Column<int>(type: "int", nullable: false),
                    fromDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateRegistrationRequestInfos", x => x.id);
                    table.ForeignKey(
                        name: "FK_PrivateRegistrationRequestInfos_PrivateRegistrationRequests_requestId",
                        column: x => x.requestId,
                        principalTable: "PrivateRegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentRequests",
                columns: table => new
                {
                    privateRegistrationRequestsid = table.Column<int>(type: "int", nullable: false),
                    studentsid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRequests", x => new { x.privateRegistrationRequestsid, x.studentsid });
                    table.ForeignKey(
                        name: "FK_StudentRequests_PrivateRegistrationRequests_privateRegistrationRequestsid",
                        column: x => x.privateRegistrationRequestsid,
                        principalTable: "PrivateRegistrationRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentRequests_Students_studentsid",
                        column: x => x.studentsid,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentFiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paymentId = table.Column<int>(type: "int", nullable: false),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentFiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_PaymentFiles_Payments_paymentId",
                        column: x => x.paymentId,
                        principalTable: "Payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamDate",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    examDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    studentId = table.Column<int>(type: "int", nullable: true),
                    privateCourseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamDate", x => x.id);
                    table.ForeignKey(
                        name: "FK_ExamDate_PrivateCourses_privateCourseId",
                        column: x => x.privateCourseId,
                        principalTable: "PrivateCourses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ExamDate_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PrivateClasses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    method = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    toTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    privateCourseId = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedBy = table.Column<int>(type: "int", nullable: true),
                    studentCancellationRequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateClasses", x => x.id);
                    table.ForeignKey(
                        name: "FK_PrivateClasses_PrivateCourses_privateCourseId",
                        column: x => x.privateCourseId,
                        principalTable: "PrivateCourses",
                        principalColumn: "id");
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
                    privateReqInfoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferredDays", x => x.id);
                    table.ForeignKey(
                        name: "FK_PreferredDays_PrivateRegistrationRequestInfos_privateReqInfoId",
                        column: x => x.privateReqInfoId,
                        principalTable: "PrivateRegistrationRequestInfos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentClassCancellationRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    privateClassId = table.Column<int>(type: "int", nullable: false),
                    studentRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OARemark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClassCancellationRequest", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentClassCancellationRequest_PrivateClasses_privateClassId",
                        column: x => x.privateClassId,
                        principalTable: "PrivateClasses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClassCancellationRequest_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentPrivateClasses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentId = table.Column<int>(type: "int", nullable: false),
                    classId = table.Column<int>(type: "int", nullable: false),
                    privateClassid = table.Column<int>(type: "int", nullable: false),
                    attendance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPrivateClasses", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentPrivateClasses_PrivateClasses_privateClassid",
                        column: x => x.privateClassid,
                        principalTable: "PrivateClasses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentPrivateClasses_Students_studentId",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherPrivateClasses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacherId = table.Column<int>(type: "int", nullable: false),
                    workType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    privateClassId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherPrivateClasses", x => x.id);
                    table.ForeignKey(
                        name: "FK_TeacherPrivateClasses_PrivateClasses_privateClassId",
                        column: x => x.privateClassId,
                        principalTable: "PrivateClasses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherPrivateClasses_Teachers_teacherId",
                        column: x => x.teacherId,
                        principalTable: "Teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_studentId",
                table: "Addresses",
                column: "studentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamDate_privateCourseId",
                table: "ExamDate",
                column: "privateCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamDate_studentId",
                table: "ExamDate",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_Parents_studentId",
                table: "Parents",
                column: "studentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentFiles_paymentId",
                table: "PaymentFiles",
                column: "paymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_privateRegReqId",
                table: "Payments",
                column: "privateRegReqId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferredDays_privateReqInfoId",
                table: "PreferredDays",
                column: "privateReqInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateClasses_privateCourseId",
                table: "PrivateClasses",
                column: "privateCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateCourses_requestid",
                table: "PrivateCourses",
                column: "requestid");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateRegistrationRequestInfos_requestId",
                table: "PrivateRegistrationRequestInfos",
                column: "requestId");

            migrationBuilder.CreateIndex(
                name: "Index_statuses",
                table: "PrivateRegistrationRequests",
                columns: new[] { "status", "EAStatus", "paymentStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateRegistrationRequests_Staffid",
                table: "PrivateRegistrationRequests",
                column: "Staffid");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdditionalFiles_studentId",
                table: "StudentAdditionalFiles",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassCancellationRequest_privateClassId",
                table: "StudentClassCancellationRequest",
                column: "privateClassId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentClassCancellationRequest_studentId",
                table: "StudentClassCancellationRequest",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "Index_attendance",
                table: "StudentPrivateClasses",
                column: "attendance");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPrivateClasses_privateClassid",
                table: "StudentPrivateClasses",
                column: "privateClassid");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPrivateClasses_studentId",
                table: "StudentPrivateClasses",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequests_studentsid",
                table: "StudentRequests",
                column: "studentsid");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLeavingRequests_teacherId",
                table: "TeacherLeavingRequests",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "Index_status",
                table: "TeacherPrivateClasses",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPrivateClasses_privateClassId",
                table: "TeacherPrivateClasses",
                column: "privateClassId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPrivateClasses_teacherId",
                table: "TeacherPrivateClasses",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimes_teacherId",
                table: "WorkTimes",
                column: "teacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "EAs");

            migrationBuilder.DropTable(
                name: "EPs");

            migrationBuilder.DropTable(
                name: "ExamDate");

            migrationBuilder.DropTable(
                name: "OAs");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropTable(
                name: "PaymentFiles");

            migrationBuilder.DropTable(
                name: "PreferredDays");

            migrationBuilder.DropTable(
                name: "StudentAdditionalFiles");

            migrationBuilder.DropTable(
                name: "StudentClassCancellationRequest");

            migrationBuilder.DropTable(
                name: "StudentPrivateClasses");

            migrationBuilder.DropTable(
                name: "StudentRequests");

            migrationBuilder.DropTable(
                name: "TeacherLeavingRequests");

            migrationBuilder.DropTable(
                name: "TeacherPrivateClasses");

            migrationBuilder.DropTable(
                name: "WorkTimes");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PrivateRegistrationRequestInfos");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "PrivateClasses");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "PrivateCourses");

            migrationBuilder.DropTable(
                name: "PrivateRegistrationRequests");

            migrationBuilder.DropTable(
                name: "Staffs");
        }
    }
}
