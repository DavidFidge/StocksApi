using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksApi.Data.Migrations
{
    public partial class AddDividends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dividend",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PortfolioId = table.Column<Guid>(nullable: true),
                    StockId = table.Column<Guid>(nullable: true),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    DividendAmount = table.Column<decimal>(nullable: false),
                    FrankedAmount = table.Column<decimal>(nullable: false),
                    IsDividendReinvestmentPlan = table.Column<bool>(nullable: false),
                    ReinvestmentNumberOfShares = table.Column<long>(nullable: false),
                    ReinvestmentPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dividend", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dividend_Portfolio_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dividend_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dividend_PortfolioId",
                table: "Dividend",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_Dividend_StockId",
                table: "Dividend",
                column: "StockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dividend");
        }
    }
}
