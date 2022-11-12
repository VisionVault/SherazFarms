using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _1611202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLines_CustomerCars_CustomerCarId",
                table: "ProductLines");

            migrationBuilder.DropIndex(
                name: "IX_ProductLines_CustomerCarId",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "CustomerCarId",
                table: "ProductLines");

            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "ProductLines",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleNumber",
                table: "ProductLines",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverName",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "VehicleNumber",
                table: "ProductLines");

            migrationBuilder.AddColumn<long>(
                name: "CustomerCarId",
                table: "ProductLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductLines_CustomerCarId",
                table: "ProductLines",
                column: "CustomerCarId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLines_CustomerCars_CustomerCarId",
                table: "ProductLines",
                column: "CustomerCarId",
                principalTable: "CustomerCars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
