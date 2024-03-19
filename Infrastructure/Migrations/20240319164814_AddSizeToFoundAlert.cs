using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeToFoundAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "FoundAnimalAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_FoundAnimalAlerts_Size_Enum",
                table: "FoundAnimalAlerts",
                sql: "\"Size\" BETWEEN 1 AND 4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FoundAnimalAlerts_Size_Enum",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "FoundAnimalAlerts");
        }
    }
}
