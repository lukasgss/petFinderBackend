using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddMissingAlerts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MissingAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OwnerPhoneNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeenLocationLatitude = table.Column<decimal>(type: "numeric(6,3)", nullable: false),
                    LastSeenLocationLongitude = table.Column<decimal>(type: "numeric(6,3)", nullable: false),
                    PetHasBeenRecovered = table.Column<bool>(type: "boolean", nullable: false),
                    PetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissingAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissingAlerts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MissingAlerts_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MissingAlerts_PetId",
                table: "MissingAlerts",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_MissingAlerts_UserId",
                table: "MissingAlerts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MissingAlerts");
        }
    }
}
