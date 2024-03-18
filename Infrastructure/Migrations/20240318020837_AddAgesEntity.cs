using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAgesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AgeInMonths",
                table: "Pets",
                newName: "AgeId");

            migrationBuilder.CreateTable(
                name: "Ages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Ages",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Filhote" },
                    { 2, "Jovem" },
                    { 3, "Adulto" },
                    { 4, "Sênior" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_AgeId",
                table: "Pets",
                column: "AgeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Ages_AgeId",
                table: "Pets",
                column: "AgeId",
                principalTable: "Ages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Ages_AgeId",
                table: "Pets");

            migrationBuilder.DropTable(
                name: "Ages");

            migrationBuilder.DropIndex(
                name: "IX_Pets_AgeId",
                table: "Pets");

            migrationBuilder.RenameColumn(
                name: "AgeId",
                table: "Pets",
                newName: "AgeInMonths");
        }
    }
}
