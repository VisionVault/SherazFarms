using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _1311202102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Cars_CarId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_UOMs_UOMId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CarId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UOMId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ForModels",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "GuardCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LeponCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MinQty",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OEM",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UOMId",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForModels",
                table: "Products",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardCode",
                table: "Products",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeponCode",
                table: "Products",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinQty",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "OEM",
                table: "Products",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UOMId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CarId",
                table: "Products",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UOMId",
                table: "Products",
                column: "UOMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Cars_CarId",
                table: "Products",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_UOMs_UOMId",
                table: "Products",
                column: "UOMId",
                principalTable: "UOMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
