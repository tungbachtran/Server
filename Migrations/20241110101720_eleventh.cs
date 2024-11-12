using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class eleventh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Faculty_facultyId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "facultyId",
                table: "User",
                newName: "FacultyId");

            migrationBuilder.RenameIndex(
                name: "IX_User_facultyId",
                table: "User",
                newName: "IX_User_FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Faculty_FacultyId",
                table: "User",
                column: "FacultyId",
                principalTable: "Faculty",
                principalColumn: "FacultyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Faculty_FacultyId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "FacultyId",
                table: "User",
                newName: "facultyId");

            migrationBuilder.RenameIndex(
                name: "IX_User_FacultyId",
                table: "User",
                newName: "IX_User_facultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Faculty_facultyId",
                table: "User",
                column: "facultyId",
                principalTable: "Faculty",
                principalColumn: "FacultyId");
        }
    }
}
