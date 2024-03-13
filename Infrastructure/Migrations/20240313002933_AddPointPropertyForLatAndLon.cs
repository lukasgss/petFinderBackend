using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointPropertyForLatAndLon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoundLocationLatitude",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "FoundLocationLongitude",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "FoundLocationLatitude",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropColumn(
                name: "FoundLocationLongitude",
                table: "FoundAnimalAlerts");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "FoundAnimalUserPreferences",
                type: "geometry",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "FoundAnimalAlerts",
                type: "geometry",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "FoundAnimalUserPreferences");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "FoundAnimalAlerts");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.AddColumn<double>(
                name: "FoundLocationLatitude",
                table: "FoundAnimalUserPreferences",
                type: "numeric(6,3)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FoundLocationLongitude",
                table: "FoundAnimalUserPreferences",
                type: "numeric(6,3)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FoundLocationLatitude",
                table: "FoundAnimalAlerts",
                type: "numeric(6,3)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FoundLocationLongitude",
                table: "FoundAnimalAlerts",
                type: "numeric(6,3)",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
