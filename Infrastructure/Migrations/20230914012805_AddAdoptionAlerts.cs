using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddAdoptionAlerts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "MissingAlerts");

            migrationBuilder.DropColumn(
                name: "OwnerPhoneNumber",
                table: "MissingAlerts");

            migrationBuilder.CreateTable(
                name: "AdoptionAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OnlyForScreenedProperties = table.Column<bool>(type: "boolean", nullable: false),
                    LocationLatitude = table.Column<decimal>(type: "numeric(6,3)", nullable: false),
                    LocationLongitude = table.Column<decimal>(type: "numeric(6,3)", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdoptionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptionAlerts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionAlerts_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAlerts_PetId",
                table: "AdoptionAlerts",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAlerts_UserId",
                table: "AdoptionAlerts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionAlerts");

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "MissingAlerts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerPhoneNumber",
                table: "MissingAlerts",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }
    }
}
