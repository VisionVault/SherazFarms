using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _2709202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EngineCC",
                table: "Cars",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineNumber",
                table: "Cars",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardNumber",
                table: "Cars",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LepponNumber",
                table: "Cars",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MannNumber",
                table: "Cars",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Cars",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEM",
                table: "Cars",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OilCapacity",
                table: "Cars",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Cars",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
