using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleImagesFoundAnimalAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Pets_Gender_Enum",
                table: "Pets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FoundAnimalAlerts_Gender_Enum",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "FoundAnimalAlerts");

            migrationBuilder.CreateTable(
                name: "FoundAnimalAlertImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FoundAnimalAlertId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundAnimalAlertImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoundAnimalAlertImages_FoundAnimalAlerts_FoundAnimalAlertId",
                        column: x => x.FoundAnimalAlertId,
                        principalTable: "FoundAnimalAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pets_Gender_Enum",
                table: "Pets",
                sql: "\"Gender\" BETWEEN 0 AND 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FoundAnimalAlerts_Gender_Enum",
                table: "FoundAnimalAlerts",
                sql: "\"Gender\" BETWEEN 0 AND 2");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlertImages_FoundAnimalAlertId",
                table: "FoundAnimalAlertImages",
                column: "FoundAnimalAlertId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoundAnimalAlertImages");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Pets_Gender_Enum",
                table: "Pets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FoundAnimalAlerts_Gender_Enum",
                table: "FoundAnimalAlerts");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "FoundAnimalAlerts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pets_Gender_Enum",
                table: "Pets",
                sql: "\"Gender\" IN (0, 1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FoundAnimalAlerts_Gender_Enum",
                table: "FoundAnimalAlerts",
                sql: "\"Gender\" IN (0, 1, 2)");
        }
    }
}
