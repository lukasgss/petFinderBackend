using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointPropertyForLatAndLonOnMissingAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSeenLocationLatitude",
                table: "MissingAlerts");

            migrationBuilder.DropColumn(
                name: "LastSeenLocationLongitude",
                table: "MissingAlerts");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "MissingAlerts",
                type: "geometry",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "MissingAlerts");

            migrationBuilder.AddColumn<double>(
                name: "LastSeenLocationLatitude",
                table: "MissingAlerts",
                type: "numeric(6,3)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LastSeenLocationLongitude",
                table: "MissingAlerts",
                type: "numeric(6,3)",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
