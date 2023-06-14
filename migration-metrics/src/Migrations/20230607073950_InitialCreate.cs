using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MigrationMetrics.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonthlyCountStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecordedTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    SourceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinationCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyCountStats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyCountStats");
        }
    }
}
