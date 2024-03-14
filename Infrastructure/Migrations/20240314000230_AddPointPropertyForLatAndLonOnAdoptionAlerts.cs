using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointPropertyForLatAndLonOnAdoptionAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoundLocationLatitude",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "FoundLocationLongitude",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "LocationLatitude",
                table: "AdoptionAlerts");

            migrationBuilder.DropColumn(
                name: "LocationLongitude",
                table: "AdoptionAlerts");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "AdoptionUserPreferences",
                type: "geometry",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "AdoptionAlerts",
                type: "geometry",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "AdoptionUserPreferences");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "AdoptionAlerts");

            migrationBuilder.AddColumn<double>(
                name: "FoundLocationLatitude",
                table: "AdoptionUserPreferences",
                type: "numeric(6,3)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FoundLocationLongitude",
                table: "AdoptionUserPreferences",
                type: "numeric(6,3)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationLatitude",
                table: "AdoptionAlerts",
                type: "numeric(6,3)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LocationLongitude",
                table: "AdoptionAlerts",
                type: "numeric(6,3)",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
