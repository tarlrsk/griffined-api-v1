using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class StudentIdPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Student_studentId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_CancellationRequest_Student_studentId",
                table: "CancellationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseMember_Student_studentId",
                table: "CourseMember");

            migrationBuilder.DropForeignKey(
                name: "FK_Parent_Student_studentId",
                table: "Parent");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationRequestMember_Student_studentId",
                table: "RegistrationRequestMember");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAdditionalFile_Student_studentId",
                table: "StudentAdditionalFile");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendance_Student_studentId",
                table: "StudentAttendance");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotification_Student_studentId",
                table: "StudentNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Student",
                newName: "autoIncrementId");

            migrationBuilder.AlterColumn<int>(
                name: "studentId",
                table: "Student",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "studentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Student_studentId",
                table: "Address",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "studentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancellationRequest_Student_studentId",
                table: "CancellationRequest",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "studentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseMember_Student_studentId",
                table: "CourseMember",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "studentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parent_Student_studentId",
                table: "Parent",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "studentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationRequestMember_Student_studentId",
                table: "RegistrationRequestMember",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "studentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAdditionalFile_Student_studentId",
                table: "StudentAdditionalFile",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "studentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendance_Student_studentId",
                table: "StudentAttendance",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "studentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotification_Student_studentId",
                table: "StudentNotification",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "studentId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Student_studentId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_CancellationRequest_Student_studentId",
                table: "CancellationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseMember_Student_studentId",
                table: "CourseMember");

            migrationBuilder.DropForeignKey(
                name: "FK_Parent_Student_studentId",
                table: "Parent");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationRequestMember_Student_studentId",
                table: "RegistrationRequestMember");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAdditionalFile_Student_studentId",
                table: "StudentAdditionalFile");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendance_Student_studentId",
                table: "StudentAttendance");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotification_Student_studentId",
                table: "StudentNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "autoIncrementId",
                table: "Student",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "studentId",
                table: "Student",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Student_studentId",
                table: "Address",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_CancellationRequest_Student_studentId",
                table: "CancellationRequest",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseMember_Student_studentId",
                table: "CourseMember",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parent_Student_studentId",
                table: "Parent",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationRequestMember_Student_studentId",
                table: "RegistrationRequestMember",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAdditionalFile_Student_studentId",
                table: "StudentAdditionalFile",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendance_Student_studentId",
                table: "StudentAttendance",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotification_Student_studentId",
                table: "StudentNotification",
                column: "studentId",
                principalTable: "Student",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
