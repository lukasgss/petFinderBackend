using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class EditMessageAndRenameTimeStampColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "UserMessages",
                newName: "TimeStampUtc");

            migrationBuilder.AddColumn<bool>(
                name: "HasBeenEdited",
                table: "UserMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBeenEdited",
                table: "UserMessages");

            migrationBuilder.RenameColumn(
                name: "TimeStampUtc",
                table: "UserMessages",
                newName: "TimeStamp");
        }
    }
}
