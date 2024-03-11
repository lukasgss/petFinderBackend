using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdoptionUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalUserPreferences_UserId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.CreateTable(
                name: "AdoptionUserPreferences",
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
                    table.PrimaryKey("PK_AdoptionUserPreferences", x => x.Id);
                    table.CheckConstraint("CK_AdoptionUserPreferences_Gender_Enum", "\"Gender\" BETWEEN 0 AND 2");
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferences_Breeds_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breeds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferences_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AdoptionUserPreferencesColor",
                columns: table => new
                {
                    AdoptionUserPreferencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColorsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionUserPreferencesColor", x => new { x.AdoptionUserPreferencesId, x.ColorsId });
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferencesColor_AdoptionUserPreferences_Adopti~",
                        column: x => x.AdoptionUserPreferencesId,
                        principalTable: "AdoptionUserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferencesColor_Colors_ColorsId",
                        column: x => x.ColorsId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferences_UserId",
                table: "FoundAnimalUserPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferences_BreedId",
                table: "AdoptionUserPreferences",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferences_SpeciesId",
                table: "AdoptionUserPreferences",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferences_UserId",
                table: "AdoptionUserPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferencesColor_ColorsId",
                table: "AdoptionUserPreferencesColor",
                column: "ColorsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionUserPreferencesColor");

            migrationBuilder.DropTable(
                name: "AdoptionUserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalUserPreferences_UserId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferences_UserId",
                table: "FoundAnimalUserPreferences",
                column: "UserId");
        }
    }
}
