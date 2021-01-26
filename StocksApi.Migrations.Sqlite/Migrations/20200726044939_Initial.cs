using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StocksApi.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioManager",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioManager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    IndustryGroup = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Portfolio",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PortfolioManagerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolio_PortfolioManager_PortfolioManagerId",
                        column: x => x.PortfolioManagerId,
                        principalTable: "PortfolioManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EndOfDay",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StockId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Open = table.Column<decimal>(nullable: false),
                    Low = table.Column<decimal>(nullable: false),
                    High = table.Column<decimal>(nullable: false),
                    Close = table.Column<decimal>(nullable: false),
                    Volume = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndOfDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndOfDay_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Holding",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StockId = table.Column<Guid>(nullable: true),
                    PurchaseDate = table.Column<DateTime>(nullable: false),
                    NumberOfShares = table.Column<long>(nullable: false),
                    PurchasePrice = table.Column<decimal>(nullable: false),
                    Brokerage = table.Column<decimal>(nullable: false),
                    PortfolioId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holding_Portfolio_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Holding_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EndOfDay_StockId",
                table: "EndOfDay",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Holding_PortfolioId",
                table: "Holding",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_Holding_StockId",
                table: "Holding",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolio_PortfolioManagerId",
                table: "Portfolio",
                column: "PortfolioManagerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EndOfDay");

            migrationBuilder.DropTable(
                name: "Holding");

            migrationBuilder.DropTable(
                name: "Portfolio");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "PortfolioManager");
        }
    }
}
