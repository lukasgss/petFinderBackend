using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddRecoveryDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetHasBeenRecovered",
                table: "MissingAlerts");

            migrationBuilder.AddColumn<DateOnly>(
                name: "RecoveryDate",
                table: "MissingAlerts",
                type: "date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoveryDate",
                table: "MissingAlerts");

            migrationBuilder.AddColumn<bool>(
                name: "PetHasBeenRecovered",
                table: "MissingAlerts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
