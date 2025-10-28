using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBeast.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToFoodItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "FoodItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodItem_UserId",
                table: "FoodItem",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodItem_Users_UserId",
                table: "FoodItem",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodItem_Users_UserId",
                table: "FoodItem");

            migrationBuilder.DropIndex(
                name: "IX_FoodItem_UserId",
                table: "FoodItem");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FoodItem");
        }
    }
}
