using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdipadapter.Data.Migrations
{
    public partial class UniqueUserParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "JustinUser",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_JustinUser_UserName",
                table: "JustinUser",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JustinUser_UserName",
                table: "JustinUser");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "JustinUser",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
