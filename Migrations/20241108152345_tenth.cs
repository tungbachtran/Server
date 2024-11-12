using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class tenth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "facultyId",
                table: "User",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_facultyId",
                table: "User",
                column: "facultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Faculty_facultyId",
                table: "User",
                column: "facultyId",
                principalTable: "Faculty",
                principalColumn: "FacultyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Faculty_facultyId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_facultyId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "facultyId",
                table: "User");
        }
    }
}
