using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _2809202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngineCC",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "EngineNumber",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "GuardNumber",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "LepponNumber",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "MannNumber",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "OEM",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "OilCapacity",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Cars");

            migrationBuilder.CreateTable(
                name: "CarBrands",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Model = table.Column<string>(maxLength: 500, nullable: true),
                    Year = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarBrands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarBrands_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarEngines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarBrandId = table.Column<int>(nullable: true),
                    EngineNumber = table.Column<string>(maxLength: 500, nullable: true),
                    EngineCC = table.Column<string>(maxLength: 50, nullable: true),
                    OilCapacity = table.Column<string>(maxLength: 50, nullable: true),
                    LepponNumber = table.Column<string>(maxLength: 100, nullable: true),
                    OEM = table.Column<string>(maxLength: 100, nullable: true),
                    GuardNumber = table.Column<string>(maxLength: 100, nullable: true),
                    MannNumber = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarEngines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarEngines_CarBrands_CarBrandId",
                        column: x => x.CarBrandId,
                        principalTable: "CarBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarBrands_CarId",
                table: "CarBrands",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarEngines_CarBrandId",
                table: "CarEngines",
                column: "CarBrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarEngines");

            migrationBuilder.DropTable(
                name: "CarBrands");

            migrationBuilder.AddColumn<string>(
                name: "EngineCC",
                table: "Cars",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineNumber",
                table: "Cars",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardNumber",
                table: "Cars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LepponNumber",
                table: "Cars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MannNumber",
                table: "Cars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Cars",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEM",
                table: "Cars",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OilCapacity",
                table: "Cars",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Cars",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
