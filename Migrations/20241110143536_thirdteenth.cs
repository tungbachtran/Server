using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class thirdteenth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_CourseClassroom_CourseClassroomCourseClassId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_User_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_CourseClassroomCourseClassId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "CourseClassroomCourseClassId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "CourseClassId",
                table: "ChatMessages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_CourseClassId",
                table: "ChatMessages",
                column: "CourseClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_CourseClassroom_CourseClassId",
                table: "ChatMessages",
                column: "CourseClassId",
                principalTable: "CourseClassroom",
                principalColumn: "CourseClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_User_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_CourseClassroom_CourseClassId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_User_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_CourseClassId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "CourseClassId",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CourseClassroomCourseClassId",
                table: "ChatMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_CourseClassroomCourseClassId",
                table: "ChatMessages",
                column: "CourseClassroomCourseClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_CourseClassroom_CourseClassroomCourseClassId",
                table: "ChatMessages",
                column: "CourseClassroomCourseClassId",
                principalTable: "CourseClassroom",
                principalColumn: "CourseClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_User_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "UserId");
        }
    }
}
