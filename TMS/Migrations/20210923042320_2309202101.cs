using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _2309202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForModels",
                table: "Products",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardCode",
                table: "Products",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeponCode",
                table: "Products",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinQty",
                table: "Products",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "OEM",
                table: "Products",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UOMId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CustomerCarId",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCars",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    RegistrationNumber = table.Column<string>(maxLength: 250, nullable: true),
                    Color = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCars", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UOMId",
                table: "Products",
                column: "UOMId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brands",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLines_CustomerCars_CustomerCarId",
                table: "ProductLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_UOMs_UOMId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "CustomerCars");

            migrationBuilder.DropIndex(
                name: "IX_Products_BrandId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UOMId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductLines_CustomerCarId",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "BrandId",
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

            migrationBuilder.DropColumn(
                name: "CustomerCarId",
                table: "ProductLines");
        }
    }
}
