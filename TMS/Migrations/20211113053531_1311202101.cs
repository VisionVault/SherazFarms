using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Migrations
{
    public partial class _1311202101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CurrentMilage",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "Delivery",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "NextDueMilage",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "ProductLines");

            migrationBuilder.AddColumn<double>(
                name: "ActualWeight",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EmptyWeight",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LoadedWeight",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "ProductLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLines_LocationId",
                table: "ProductLines",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLines_Locations_LocationId",
                table: "ProductLines",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLines_Locations_LocationId",
                table: "ProductLines");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_ProductLines_LocationId",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "ActualWeight",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "EmptyWeight",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "LoadedWeight",
                table: "ProductLines");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "ProductLines");

            migrationBuilder.AddColumn<long>(
                name: "BrokerId",
                table: "ProductLines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Commission",
                table: "ProductLines",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentMilage",
                table: "ProductLines",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Delivery",
                table: "ProductLines",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextDueMilage",
                table: "ProductLines",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "ProductLines",
                type: "nvarchar(50)",
                maxLength: 50,
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
    }
}
