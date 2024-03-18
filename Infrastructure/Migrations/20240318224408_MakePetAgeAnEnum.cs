using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakePetAgeAnEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionUserPreferences_Ages_AgeId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_FoundAnimalAlerts_Ages_AgeId",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_FoundAnimalUserPreferences_Ages_AgeId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Ages_AgeId",
                table: "Pets");

            migrationBuilder.DropTable(
                name: "Ages");

            migrationBuilder.DropIndex(
                name: "IX_Pets_AgeId",
                table: "Pets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Pets_Size_Enum",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalUserPreferences_AgeId",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalAlerts_AgeId",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionUserPreferences_AgeId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "AgeId",
                table: "Pets");

            migrationBuilder.RenameColumn(
                name: "AgeId",
                table: "FoundAnimalUserPreferences",
                newName: "Age");

            migrationBuilder.RenameColumn(
                name: "AgeId",
                table: "FoundAnimalAlerts",
                newName: "Age");

            migrationBuilder.RenameColumn(
                name: "AgeId",
                table: "AdoptionUserPreferences",
                newName: "Age");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Pets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pets_Age_Enum",
                table: "Pets",
                sql: "\"Age\" BETWEEN 1 AND 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pets_Size_Enum",
                table: "Pets",
                sql: "\"Size\" BETWEEN 1 AND 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FoundAnimalUserPreferences_Age_Enum",
                table: "FoundAnimalUserPreferences",
                sql: "\"Age\" BETWEEN 1 AND 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FoundAnimalAlerts_Age_Enum",
                table: "FoundAnimalAlerts",
                sql: "\"Age\" BETWEEN 1 AND 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AdoptionUserPreferences_Age_Enum",
                table: "AdoptionUserPreferences",
                sql: "\"Age\" BETWEEN 1 AND 4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Pets_Age_Enum",
                table: "Pets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Pets_Size_Enum",
                table: "Pets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FoundAnimalUserPreferences_Age_Enum",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FoundAnimalAlerts_Age_Enum",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AdoptionUserPreferences_Age_Enum",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Pets");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "FoundAnimalUserPreferences",
                newName: "AgeId");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "FoundAnimalAlerts",
                newName: "AgeId");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "AdoptionUserPreferences",
                newName: "AgeId");

            migrationBuilder.AddColumn<int>(
                name: "AgeId",
                table: "Pets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Ages",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Filhote" },
                    { 2, "Jovem" },
                    { 3, "Adulto" },
                    { 4, "Sênior" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_AgeId",
                table: "Pets",
                column: "AgeId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pets_Size_Enum",
                table: "Pets",
                sql: "\"Size\" BETWEEN 0 AND 3");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalUserPreferences_AgeId",
                table: "FoundAnimalUserPreferences",
                column: "AgeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_AgeId",
                table: "FoundAnimalAlerts",
                column: "AgeId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferences_AgeId",
                table: "AdoptionUserPreferences",
                column: "AgeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionUserPreferences_Ages_AgeId",
                table: "AdoptionUserPreferences",
                column: "AgeId",
                principalTable: "Ages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoundAnimalAlerts_Ages_AgeId",
                table: "FoundAnimalAlerts",
                column: "AgeId",
                principalTable: "Ages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoundAnimalUserPreferences_Ages_AgeId",
                table: "FoundAnimalUserPreferences",
                column: "AgeId",
                principalTable: "Ages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Ages_AgeId",
                table: "Pets",
                column: "AgeId",
                principalTable: "Ages",
                principalColumn: "Id");
        }
    }
}
