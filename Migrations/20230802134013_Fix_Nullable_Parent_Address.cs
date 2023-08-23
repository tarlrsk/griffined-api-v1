using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace griffinedapi.Migrations
{
    /// <inheritdoc />
    public partial class FixNullableParentAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parent_Student_StudentId",
                table: "Parent");

            migrationBuilder.DropIndex(
                name: "IX_Parent_StudentId",
                table: "Parent");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Parent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Parent_StudentId",
                table: "Parent",
                column: "StudentId",
                unique: true,
                filter: "[StudentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Parent_Student_StudentId",
                table: "Parent",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parent_Student_StudentId",
                table: "Parent");

            migrationBuilder.DropIndex(
                name: "IX_Parent_StudentId",
                table: "Parent");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Parent",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parent_StudentId",
                table: "Parent",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Parent_Student_StudentId",
                table: "Parent",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
