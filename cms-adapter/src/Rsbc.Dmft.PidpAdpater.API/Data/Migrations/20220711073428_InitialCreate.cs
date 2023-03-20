using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdipadapter.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryLookup",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryLookup", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "JustinAgency",
                columns: table => new
                {
                    AgencyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JustinAgency", x => x.AgencyId);
                });

            migrationBuilder.CreateTable(
                name: "JustinIdentityProvider",
                columns: table => new
                {
                    IdentityProviderId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Keycloak_idp_alias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TokenUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JustinIdentityProvider", x => x.IdentityProviderId);
                });

            migrationBuilder.CreateTable(
                name: "JustinRole",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JustinRole", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "PartyTypeLookup",
                columns: table => new
                {
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyTypeLookup", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShirtNo = table.Column<int>(type: "int", nullable: false),
                    Apperance = table.Column<int>(type: "int", nullable: false),
                    Goals = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Province", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "JustinAgencyAssignment",
                columns: table => new
                {
                    AgencyAssignmentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgencyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JustinAgencyAssignment", x => x.AgencyAssignmentId);
                    table.ForeignKey(
                        name: "FK_JustinAgencyAssignment_JustinAgency_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "JustinAgency",
                        principalColumn: "AgencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JustinAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProvinceCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postal = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JustinAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JustinAddress_CountryLookup_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "CountryLookup",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JustinAddress_Province_ProvinceCode",
                        column: x => x.ProvinceCode,
                        principalTable: "Province",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JustinPerson",
                columns: table => new
                {
                    PersonId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MiddleNames = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameSuffix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreferredName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AddressComment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JustinPerson", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_JustinPerson_JustinAddress_AddressId",
                        column: x => x.AddressId,
                        principalTable: "JustinAddress",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JustinUser",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParticipantId = table.Column<long>(type: "bigint", nullable: false),
                    DigitalIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AgencyId = table.Column<long>(type: "bigint", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    IdentityProviderId = table.Column<long>(type: "bigint", nullable: true),
                    PartyTypeCode = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JustinUser", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_JustinUser_JustinAgency_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "JustinAgency",
                        principalColumn: "AgencyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JustinUser_JustinIdentityProvider_IdentityProviderId",
                        column: x => x.IdentityProviderId,
                        principalTable: "JustinIdentityProvider",
                        principalColumn: "IdentityProviderId");
                    table.ForeignKey(
                        name: "FK_JustinUser_JustinPerson_PersonId",
                        column: x => x.PersonId,
                        principalTable: "JustinPerson",
                        principalColumn: "PersonId");
                    table.ForeignKey(
                        name: "FK_JustinUser_PartyTypeLookup_PartyTypeCode",
                        column: x => x.PartyTypeCode,
                        principalTable: "PartyTypeLookup",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "JustinUserRole",
                columns: table => new
                {
                    UserRoleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JustinUserRole", x => x.UserRoleId);
                    table.ForeignKey(
                        name: "FK_JustinUserRole_JustinRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "JustinRole",
                        principalColumn: "RoleId");
                    table.ForeignKey(
                        name: "FK_JustinUserRole_JustinUser_UserId",
                        column: x => x.UserId,
                        principalTable: "JustinUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.InsertData(
                table: "CountryLookup",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { "CA", "Canada" },
                    { "US", "United States" }
                });

            migrationBuilder.InsertData(
                table: "JustinRole",
                columns: new[] { "RoleId", "Created", "Description", "IsDisabled", "IsPublic", "Modified", "Name" },
                values: new object[,]
                {
                    { 1L, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Super Users", false, false, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator" },
                    { 2L, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BCPS Users", false, false, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BCPS" },
                    { 3L, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Defence Users", false, false, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Defence Council" },
                    { 4L, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Police Users", false, false, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Police" },
                    { 5L, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Accused Users", false, false, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Accused" },
                    { 6L, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OutofCustody Users", false, false, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OutofCustody" }
                });

            migrationBuilder.InsertData(
                table: "PartyTypeLookup",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { 1, "Organization" },
                    { 2, "Individual" },
                    { 3, "Staff" }
                });

            migrationBuilder.InsertData(
                table: "Province",
                columns: new[] { "Code", "CountryCode", "Name" },
                values: new object[,]
                {
                    { "AB", "CA", "Alberta" },
                    { "AK", "US", "Alaska" },
                    { "AL", "US", "Alabama" },
                    { "AR", "US", "Arkansas" },
                    { "AS", "US", "American Samoa" },
                    { "AZ", "US", "Arizona" },
                    { "BC", "CA", "British Columbia" },
                    { "CA", "US", "California" },
                    { "CO", "US", "Colorado" },
                    { "CT", "US", "Connecticut" },
                    { "DC", "US", "District of Columbia" },
                    { "DE", "US", "Delaware" },
                    { "FL", "US", "Florida" },
                    { "GA", "US", "Georgia" },
                    { "GU", "US", "Guam" },
                    { "HI", "US", "Hawaii" },
                    { "IA", "US", "Iowa" },
                    { "ID", "US", "Idaho" },
                    { "IL", "US", "Illinois" },
                    { "IN", "US", "Indiana" },
                    { "KS", "US", "Kansas" },
                    { "KY", "US", "Kentucky" },
                    { "LA", "US", "Louisiana" },
                    { "MA", "US", "Massachusetts" },
                    { "MB", "CA", "Manitoba" },
                    { "MD", "US", "Maryland" },
                    { "ME", "US", "Maine" },
                    { "MI", "US", "Michigan" },
                    { "MN", "US", "Minnesota" },
                    { "MO", "US", "Missouri" },
                    { "MP", "US", "Northern Mariana Islands" }
                });

            migrationBuilder.InsertData(
                table: "Province",
                columns: new[] { "Code", "CountryCode", "Name" },
                values: new object[,]
                {
                    { "MS", "US", "Mississippi" },
                    { "MT", "US", "Montana" },
                    { "NB", "CA", "New Brunswick" },
                    { "NC", "US", "North Carolina" },
                    { "ND", "US", "North Dakota" },
                    { "NE", "US", "Nebraska" },
                    { "NH", "US", "New Hampshire" },
                    { "NJ", "US", "New Jersey" },
                    { "NL", "CA", "Newfoundland and Labrador" },
                    { "NM", "US", "New Mexico" },
                    { "NS", "CA", "Nova Scotia" },
                    { "NT", "CA", "Northwest Territories" },
                    { "NU", "CA", "Nunavut" },
                    { "NV", "US", "Nevada" },
                    { "NY", "US", "New York" },
                    { "OH", "US", "Ohio" },
                    { "OK", "US", "Oklahoma" },
                    { "ON", "CA", "Ontario" },
                    { "OR", "US", "Oregon" },
                    { "PA", "US", "Pennsylvania" },
                    { "PE", "CA", "Prince Edward Island" },
                    { "PR", "US", "Puerto Rico" },
                    { "QC", "CA", "Quebec" },
                    { "RI", "US", "Rhode Island" },
                    { "SC", "US", "South Carolina" },
                    { "SD", "US", "South Dakota" },
                    { "SK", "CA", "Saskatchewan" },
                    { "TN", "US", "Tennessee" },
                    { "TX", "US", "Texas" },
                    { "UM", "US", "United States Minor Outlying Islands" },
                    { "UT", "US", "Utah" },
                    { "VA", "US", "Virginia" },
                    { "VI", "US", "Virgin Islands, U.S." },
                    { "VT", "US", "Vermont" },
                    { "WA", "US", "Washington" },
                    { "WI", "US", "Wisconsin" },
                    { "WV", "US", "West Virginia" },
                    { "WY", "US", "Wyoming" },
                    { "YT", "CA", "Yukon" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JustinAddress_CountryCode",
                table: "JustinAddress",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_JustinAddress_ProvinceCode",
                table: "JustinAddress",
                column: "ProvinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_JustinAgencyAssignment_AgencyId",
                table: "JustinAgencyAssignment",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_JustinPerson_AddressId",
                table: "JustinPerson",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_JustinUser_AgencyId",
                table: "JustinUser",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_JustinUser_IdentityProviderId",
                table: "JustinUser",
                column: "IdentityProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_JustinUser_ParticipantId",
                table: "JustinUser",
                column: "ParticipantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JustinUser_PartyTypeCode",
                table: "JustinUser",
                column: "PartyTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_JustinUser_PersonId",
                table: "JustinUser",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JustinUserRole_RoleId",
                table: "JustinUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_JustinUserRole_UserId",
                table: "JustinUserRole",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JustinAgencyAssignment");

            migrationBuilder.DropTable(
                name: "JustinUserRole");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "JustinRole");

            migrationBuilder.DropTable(
                name: "JustinUser");

            migrationBuilder.DropTable(
                name: "JustinAgency");

            migrationBuilder.DropTable(
                name: "JustinIdentityProvider");

            migrationBuilder.DropTable(
                name: "JustinPerson");

            migrationBuilder.DropTable(
                name: "PartyTypeLookup");

            migrationBuilder.DropTable(
                name: "JustinAddress");

            migrationBuilder.DropTable(
                name: "CountryLookup");

            migrationBuilder.DropTable(
                name: "Province");
        }
    }
}
