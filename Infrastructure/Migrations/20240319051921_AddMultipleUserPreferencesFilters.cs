using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleUserPreferencesFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionUserPreferences_Breeds_BreedId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionUserPreferences_Species_SpeciesId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_FoundAnimalUserPreferences_Breeds_BreedId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_FoundAnimalUserPreferences_Species_SpeciesId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalUserPreferences_BreedId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalUserPreferences_SpeciesId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FoundAnimalUserPreferences_Age_Enum",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FoundAnimalUserPreferences_Gender_Enum",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionUserPreferences_BreedId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionUserPreferences_SpeciesId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AdoptionUserPreferences_Age_Enum",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AdoptionUserPreferences_Gender_Enum",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "BreedId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "SpeciesId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "BreedId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "SpeciesId",
                table: "AdoptionUserPreferences");

            migrationBuilder.AddColumn<int[]>(
                name: "Ages",
                table: "FoundAnimalUserPreferences",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "Genders",
                table: "FoundAnimalUserPreferences",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "Sizes",
                table: "FoundAnimalUserPreferences",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "Ages",
                table: "AdoptionUserPreferences",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "Genders",
                table: "AdoptionUserPreferences",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "Sizes",
                table: "AdoptionUserPreferences",
                type: "integer[]",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdoptionUserPreferencesBreed",
                columns: table => new
                {
                    AdoptionUserPreferencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    BreedsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionUserPreferencesBreed", x => new { x.AdoptionUserPreferencesId, x.BreedsId });
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferencesBreed_AdoptionUserPreferences_Adopti~",
                        column: x => x.AdoptionUserPreferencesId,
                        principalTable: "AdoptionUserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferencesBreed_Breeds_BreedsId",
                        column: x => x.BreedsId,
                        principalTable: "Breeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdoptionUserPreferencesSpecies",
                columns: table => new
                {
                    AdoptionUserPreferencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionUserPreferencesSpecies", x => new { x.AdoptionUserPreferencesId, x.SpeciesId });
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferencesSpecies_AdoptionUserPreferences_Adop~",
                        column: x => x.AdoptionUserPreferencesId,
                        principalTable: "AdoptionUserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionUserPreferencesSpecies_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BreedFoundAnimalUserPreferences",
                columns: table => new
                {
                    BreedsId = table.Column<int>(type: "integer", nullable: false),
                    FoundAnimalUserPreferencesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreedFoundAnimalUserPreferences", x => new { x.BreedsId, x.FoundAnimalUserPreferencesId });
                    table.ForeignKey(
                        name: "FK_BreedFoundAnimalUserPreferences_Breeds_BreedsId",
                        column: x => x.BreedsId,
                        principalTable: "Breeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BreedFoundAnimalUserPreferences_FoundAnimalUserPreferences_~",
                        column: x => x.FoundAnimalUserPreferencesId,
                        principalTable: "FoundAnimalUserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoundAnimalUserPreferencesSpecies",
                columns: table => new
                {
                    FoundAnimalUserPreferencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundAnimalUserPreferencesSpecies", x => new { x.FoundAnimalUserPreferencesId, x.SpeciesId });
                    table.ForeignKey(
                        name: "FK_FoundAnimalUserPreferencesSpecies_FoundAnimalUserPreference~",
                        column: x => x.FoundAnimalUserPreferencesId,
                        principalTable: "FoundAnimalUserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoundAnimalUserPreferencesSpecies_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferencesBreed_BreedsId",
                table: "AdoptionUserPreferencesBreed",
                column: "BreedsId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferencesSpecies_SpeciesId",
                table: "AdoptionUserPreferencesSpecies",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_BreedFoundAnimalUserPreferences_FoundAnimalUserPreferencesId",
                table: "BreedFoundAnimalUserPreferences",
                column: "FoundAnimalUserPreferencesId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferencesSpecies_SpeciesId",
                table: "FoundAnimalUserPreferencesSpecies",
                column: "SpeciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionUserPreferencesBreed");

            migrationBuilder.DropTable(
                name: "AdoptionUserPreferencesSpecies");

            migrationBuilder.DropTable(
                name: "BreedFoundAnimalUserPreferences");

            migrationBuilder.DropTable(
                name: "FoundAnimalUserPreferencesSpecies");

            migrationBuilder.DropColumn(
                name: "Ages",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "Genders",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "Sizes",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "Ages",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "Genders",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "Sizes",
                table: "AdoptionUserPreferences");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "FoundAnimalUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreedId",
                table: "FoundAnimalUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "FoundAnimalUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpeciesId",
                table: "FoundAnimalUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "AdoptionUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreedId",
                table: "AdoptionUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AdoptionUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpeciesId",
                table: "AdoptionUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferences_BreedId",
                table: "FoundAnimalUserPreferences",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferences_SpeciesId",
                table: "FoundAnimalUserPreferences",
                column: "SpeciesId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FoundAnimalUserPreferences_Age_Enum",
                table: "FoundAnimalUserPreferences",
                sql: "\"Age\" BETWEEN 1 AND 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FoundAnimalUserPreferences_Gender_Enum",
                table: "FoundAnimalUserPreferences",
                sql: "\"Gender\" BETWEEN 0 AND 2");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferences_BreedId",
                table: "AdoptionUserPreferences",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferences_SpeciesId",
                table: "AdoptionUserPreferences",
                column: "SpeciesId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AdoptionUserPreferences_Age_Enum",
                table: "AdoptionUserPreferences",
                sql: "\"Age\" BETWEEN 1 AND 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AdoptionUserPreferences_Gender_Enum",
                table: "AdoptionUserPreferences",
                sql: "\"Gender\" BETWEEN 0 AND 2");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionUserPreferences_Breeds_BreedId",
                table: "AdoptionUserPreferences",
                column: "BreedId",
                principalTable: "Breeds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionUserPreferences_Species_SpeciesId",
                table: "AdoptionUserPreferences",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoundAnimalUserPreferences_Breeds_BreedId",
                table: "FoundAnimalUserPreferences",
                column: "BreedId",
                principalTable: "Breeds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoundAnimalUserPreferences_Species_SpeciesId",
                table: "FoundAnimalUserPreferences",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id");
        }
    }
}
