using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddAnimalVaccinations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vaccines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PetVaccine",
                columns: table => new
                {
                    PetsId = table.Column<Guid>(type: "uuid", nullable: false),
                    VaccinesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetVaccine", x => new { x.PetsId, x.VaccinesId });
                    table.ForeignKey(
                        name: "FK_PetVaccine_Pets_PetsId",
                        column: x => x.PetsId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetVaccine_Vaccines_VaccinesId",
                        column: x => x.VaccinesId,
                        principalTable: "Vaccines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesVaccine",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    VaccinesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesVaccine", x => new { x.SpeciesId, x.VaccinesId });
                    table.ForeignKey(
                        name: "FK_SpeciesVaccine_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeciesVaccine_Vaccines_VaccinesId",
                        column: x => x.VaccinesId,
                        principalTable: "Vaccines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PetVaccine_VaccinesId",
                table: "PetVaccine",
                column: "VaccinesId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesVaccine_VaccinesId",
                table: "SpeciesVaccine",
                column: "VaccinesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetVaccine");

            migrationBuilder.DropTable(
                name: "SpeciesVaccine");

            migrationBuilder.DropTable(
                name: "Vaccines");
        }
    }
}
