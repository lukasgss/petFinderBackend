using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFoundAnimalAlertNotificationsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoundAnimalAlertNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundAnimalAlertId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundAnimalAlertNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoundAnimalAlertNotifications_FoundAnimalAlerts_FoundAnimal~",
                        column: x => x.FoundAnimalAlertId,
                        principalTable: "FoundAnimalAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoundAnimalAlertNotificationsUser",
                columns: table => new
                {
                    FoundAnimalAlertNotificationsId = table.Column<long>(type: "bigint", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundAnimalAlertNotificationsUser", x => new { x.FoundAnimalAlertNotificationsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_FoundAnimalAlertNotificationsUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoundAnimalAlertNotificationsUser_FoundAnimalAlertNotificat~",
                        column: x => x.FoundAnimalAlertNotificationsId,
                        principalTable: "FoundAnimalAlertNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlertNotifications_FoundAnimalAlertId",
                table: "FoundAnimalAlertNotifications",
                column: "FoundAnimalAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlertNotificationsUser_UsersId",
                table: "FoundAnimalAlertNotificationsUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoundAnimalAlertNotificationsUser");

            migrationBuilder.DropTable(
                name: "FoundAnimalAlertNotifications");
        }
    }
}
