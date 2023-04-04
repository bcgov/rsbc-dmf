using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdipadapter.Data.Migrations
{
    public partial class PersonGender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "JustinPerson",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "JustinPerson");
        }
    }
}
