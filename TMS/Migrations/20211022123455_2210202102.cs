using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _2210202102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "AccountDetails",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AccountDetails",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "AccountDetails");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AccountDetails");
        }
    }
}
