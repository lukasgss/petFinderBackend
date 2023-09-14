using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddPetGenderAndAge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgeInMonths",
                table: "Pets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Pets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Pets_Gender_Enum",
                table: "Pets",
                sql: "\"Gender\" IN (0, 1, 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Pets_Gender_Enum",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "AgeInMonths",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Pets");
        }
    }
}
