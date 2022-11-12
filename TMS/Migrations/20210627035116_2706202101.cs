using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _2706202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "BillDiscount",
                table: "ProductLines",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "BillDiscountP",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountP",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Net",
                table: "ProductLines",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillDiscountP",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "DiscountP",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "Net",
                table: "ProductLines");

            migrationBuilder.AlterColumn<double>(
                name: "BillDiscount",
                table: "ProductLines",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
