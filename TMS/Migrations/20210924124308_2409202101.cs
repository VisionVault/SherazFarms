using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _2409202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarId",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CarId",
                table: "Products",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Cars_CarId",
                table: "Products",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Cars_CarId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CarId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "Products");
        }
    }
}
