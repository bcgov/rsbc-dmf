using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdipadapter.Data.Migrations
{
    public partial class JustinAgencyConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "JustinAgency",
                columns: new[] { "AgencyId", "AgencyCode", "Created", "Description", "Modified", "Name" },
                values: new object[,]
                {
                    { 1L, "SPD", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sannich Police Department" },
                    { 2L, "VICPD", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Victoria Police Department" },
                    { 3L, "DPD", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Delta Police Department" },
                    { 4L, "VPD", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vancouver Police Department" },
                    { 5L, "RCMP", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Royal Canada Mount Police" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "JustinAgency",
                keyColumn: "AgencyId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "JustinAgency",
                keyColumn: "AgencyId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "JustinAgency",
                keyColumn: "AgencyId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "JustinAgency",
                keyColumn: "AgencyId",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "JustinAgency",
                keyColumn: "AgencyId",
                keyValue: 5L);
        }
    }
}
