using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAgeToFoundAnimalUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdoptionUserPreferences_AgeId",
                table: "AdoptionUserPreferences");

            migrationBuilder.AddColumn<int>(
                name: "AgeId",
                table: "FoundAnimalUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgeId",
                table: "FoundAnimalAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoundAnimalAlerts_Ages_AgeId",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_FoundAnimalUserPreferences_Ages_AgeId",
                table: "FoundAnimalUserPreferences");

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
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "AgeId",
                table: "FoundAnimalAlerts");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferences_AgeId",
                table: "AdoptionUserPreferences",
                column: "AgeId");
        }
    }
}
