using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _0310202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CustomerCars");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCars_AccountId",
                table: "CustomerCars",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCars_Accounts_AccountId",
                table: "CustomerCars",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCars_Accounts_AccountId",
                table: "CustomerCars");

            migrationBuilder.DropIndex(
                name: "IX_CustomerCars_AccountId",
                table: "CustomerCars");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CustomerCars",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
