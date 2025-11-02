using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBeast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExperienceToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Experience",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XpToNextLevel",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "XpToNextLevel",
                table: "Pets");
        }
    }
}
