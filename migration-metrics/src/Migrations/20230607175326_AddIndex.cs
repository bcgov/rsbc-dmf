using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MigrationMetrics.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MonthlyCountStats_RecordedTime",
                table: "MonthlyCountStats",
                column: "RecordedTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MonthlyCountStats_RecordedTime",
                table: "MonthlyCountStats");
        }
    }
}
