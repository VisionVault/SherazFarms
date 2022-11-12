using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _2210202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuardNumber",
                table: "CarEngines");

            migrationBuilder.DropColumn(
                name: "LepponNumber",
                table: "CarEngines");

            migrationBuilder.DropColumn(
                name: "MannNumber",
                table: "CarEngines");

            migrationBuilder.DropColumn(
                name: "OEM",
                table: "CarEngines");

            migrationBuilder.AddColumn<string>(
                name: "AirFilter",
                table: "CarEngines",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CabinFilter",
                table: "CarEngines",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "CarEngines",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuelFilter",
                table: "CarEngines",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OilFilter",
                table: "CarEngines",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AirFilter",
                table: "CarEngines");

            migrationBuilder.DropColumn(
                name: "CabinFilter",
                table: "CarEngines");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "CarEngines");

            migrationBuilder.DropColumn(
                name: "FuelFilter",
                table: "CarEngines");

            migrationBuilder.DropColumn(
                name: "OilFilter",
                table: "CarEngines");

            migrationBuilder.AddColumn<string>(
                name: "GuardNumber",
                table: "CarEngines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LepponNumber",
                table: "CarEngines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MannNumber",
                table: "CarEngines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEM",
                table: "CarEngines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
