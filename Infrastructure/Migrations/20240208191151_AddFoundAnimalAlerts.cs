using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddFoundAnimalAlerts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoundAnimalAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FoundLocationLatitude = table.Column<double>(type: "numeric(6,3)", nullable: false),
                    FoundLocationLongitude = table.Column<double>(type: "numeric(6,3)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HasBeenRecovered = table.Column<bool>(type: "boolean", nullable: false),
                    Image = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    BreedId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundAnimalAlerts", x => x.Id);
                    table.CheckConstraint("CK_FoundAnimalAlerts_Gender_Enum", "\"Gender\" IN (0, 1, 2)");
                    table.ForeignKey(
                        name: "FK_FoundAnimalAlerts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoundAnimalAlerts_Breeds_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breeds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FoundAnimalAlerts_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColorFoundAnimalAlert",
                columns: table => new
                {
                    ColorsId = table.Column<int>(type: "integer", nullable: false),
                    FoundAnimalAlertsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorFoundAnimalAlert", x => new { x.ColorsId, x.FoundAnimalAlertsId });
                    table.ForeignKey(
                        name: "FK_ColorFoundAnimalAlert_Colors_ColorsId",
                        column: x => x.ColorsId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorFoundAnimalAlert_FoundAnimalAlerts_FoundAnimalAlertsId",
                        column: x => x.FoundAnimalAlertsId,
                        principalTable: "FoundAnimalAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorFoundAnimalAlert_FoundAnimalAlertsId",
                table: "ColorFoundAnimalAlert",
                column: "FoundAnimalAlertsId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_BreedId",
                table: "FoundAnimalAlerts",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_SpeciesId",
                table: "FoundAnimalAlerts",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_UserId",
                table: "FoundAnimalAlerts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorFoundAnimalAlert");

            migrationBuilder.DropTable(
                name: "FoundAnimalAlerts");
        }
    }
}
