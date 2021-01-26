using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksApi.Data.Migrations
{
    public partial class AddHolderIdentificationNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HolderIdentificationNumber",
                table: "Portfolio",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HolderIdentificationNumber",
                table: "Portfolio");
        }
    }
}
