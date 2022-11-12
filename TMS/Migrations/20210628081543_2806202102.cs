using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _2806202102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BrokerId",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Commission",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductLines_BrokerId",
                table: "ProductLines",
                column: "BrokerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLines_Accounts_BrokerId",
                table: "ProductLines",
                column: "BrokerId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLines_Accounts_BrokerId",
                table: "ProductLines");

            migrationBuilder.DropIndex(
                name: "IX_ProductLines_BrokerId",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "BrokerId",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "Commission",
                table: "ProductLines");
        }
    }
}
