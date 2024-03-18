using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAgeToAdoptionUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgeId",
                table: "AdoptionUserPreferences",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionUserPreferences_AgeId",
                table: "AdoptionUserPreferences",
                column: "AgeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionUserPreferences_Ages_AgeId",
                table: "AdoptionUserPreferences",
                column: "AgeId",
                principalTable: "Ages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionUserPreferences_Ages_AgeId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionUserPreferences_AgeId",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "AgeId",
                table: "AdoptionUserPreferences");
        }
    }
}
