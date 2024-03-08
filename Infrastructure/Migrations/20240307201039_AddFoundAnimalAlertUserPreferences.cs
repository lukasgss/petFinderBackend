using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFoundAnimalAlertUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoundAnimalUserPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoundLocationLatitude = table.Column<double>(type: "numeric(6,3)", nullable: true),
                    FoundLocationLongitude = table.Column<double>(type: "numeric(6,3)", nullable: true),
                    RadiusDistanceInKm = table.Column<double>(type: "double precision", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    SpeciesId = table.Column<int>(type: "integer", nullable: true),
                    BreedId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundAnimalUserPreferences", x => x.Id);
                    table.CheckConstraint("CK_FoundAnimalUserPreferences_Gender_Enum", "\"Gender\" BETWEEN 0 AND 2");
                    table.ForeignKey(
                        name: "FK_FoundAnimalUserPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoundAnimalUserPreferences_Breeds_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breeds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FoundAnimalUserPreferences_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ColorFoundAnimalUserPreferences",
                columns: table => new
                {
                    ColorsId = table.Column<int>(type: "integer", nullable: false),
                    FoundAnimalUserPreferencesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorFoundAnimalUserPreferences", x => new { x.ColorsId, x.FoundAnimalUserPreferencesId });
                    table.ForeignKey(
                        name: "FK_ColorFoundAnimalUserPreferences_Colors_ColorsId",
                        column: x => x.ColorsId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorFoundAnimalUserPreferences_FoundAnimalUserPreferences_~",
                        column: x => x.FoundAnimalUserPreferencesId,
                        principalTable: "FoundAnimalUserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorFoundAnimalUserPreferences_FoundAnimalUserPreferencesId",
                table: "ColorFoundAnimalUserPreferences",
                column: "FoundAnimalUserPreferencesId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferences_BreedId",
                table: "FoundAnimalUserPreferences",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferences_SpeciesId",
                table: "FoundAnimalUserPreferences",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferences_UserId",
                table: "FoundAnimalUserPreferences",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorFoundAnimalUserPreferences");

            migrationBuilder.DropTable(
                name: "FoundAnimalUserPreferences");
        }
    }
}
