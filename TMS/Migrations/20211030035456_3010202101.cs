using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _3010202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentMilage",
                table: "ProductLines",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextDueMilage",
                table: "ProductLines",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "ProductLines",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentMilage",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "NextDueMilage",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "ProductLines");
        }
    }
}
