using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ChangeRecoveryColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBeenRecovered",
                table: "FoundAnimalAlerts");

            migrationBuilder.AddColumn<DateOnly>(
                name: "RecoveryDate",
                table: "FoundAnimalAlerts",
                type: "date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoveryDate",
                table: "FoundAnimalAlerts");

            migrationBuilder.AddColumn<bool>(
                name: "HasBeenRecovered",
                table: "FoundAnimalAlerts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
