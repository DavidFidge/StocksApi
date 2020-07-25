using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using StocksApi.Data;
using StocksApi.Model;
using StocksApi.Service.Companies;

namespace StocksApi.Service.Tests
{
    [TestClass]
    public class CompanyInformationTests : BaseSqlLiteTest<CompanyInformation, StocksContext>
    {
        private ICompanyInformationStore _companyInformationStore;
        private CompanyInformation _companyInformation;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            using var stocksContext = new StocksContext(ContextOptions);

            SetupDatabase(stocksContext);

            _companyInformationStore = Substitute.For<ICompanyInformationStore>();

            _companyInformation = new CompanyInformation(Logger, _companyInformationStore);
        }

        [TestMethod]
        public async Task Update_Should_Add_New_Stocks()
        {
            // Arrange
            var asxData =
                "ASX listed companies as at Sat Jun 13 20:40:46 AEST 2020\r\n\r\nCompany name,ASX code,GICS industry group\r\n\"MOQ LIMITED\",\"MOQ\",\"Software & Services\"\r\n\"1300 SMILES LIMITED\",\"ONT\",\"Health Care Equipment & Services\"\r\n\"1414 DEGREES LIMITED\",\"14D\",\"Capital Goods\"\r\n";
            _companyInformationStore
                .GetFromStore()
                .Returns(asxData);

            // Act
            using (var actStocksContext = new StocksContext(ContextOptions))
            {
                await _companyInformation.Update(actStocksContext);
            }

            // Assert
            using var stocksContext = new StocksContext(ContextOptions);

            var stocks = stocksContext.Stock.ToList();

            Assert.AreEqual(3, stocks.Count);

            AssertCompanyData(stocks[0], "MOQ", "MOQ LIMITED", "Software & Services");
            AssertCompanyData(stocks[1], "ONT", "1300 SMILES LIMITED", "Health Care Equipment & Services");
            AssertCompanyData(stocks[2], "14D", "1414 DEGREES LIMITED", "Capital Goods");
        }

        [TestMethod]
        public async Task Update_Should_Update_Existing_Stocks()
        {
            // Arrange
            var arrangeStocks = new List<Stock>
            {
                new Stock
                {
                    Code = "MOQ",
                    CompanyName = "Old Company Name",
                    IndustryGroup = "Old Industry Group"
                }
            };

            SeedDatabase(arrangeStocks);

            var asxData =
                "ASX listed companies as at Sat Jun 13 20:40:46 AEST 2020\r\n\r\nCompany name,ASX code,GICS industry group\r\n\"New Company Name\",\"MOQ\",\"New Industry Group\"\r\n";

            _companyInformationStore
                .GetFromStore()
                .Returns(asxData);

            // Act
            using (var actStocksContext = new StocksContext(ContextOptions))
            {
                await _companyInformation.Update(actStocksContext);
            }

            // Assert
            using var stocksContext = new StocksContext(ContextOptions);

            var stocks = stocksContext.Stock.ToList();

            Assert.AreEqual(1, stocks.Count);
            AssertCompanyData(stocks[0], "MOQ", "New Company Name", "New Industry Group");
        }

        private void SeedDatabase(List<Stock> stocks)
        {
            using var stocksContext = new StocksContext(ContextOptions);

            stocksContext.AddRange(stocks);
            stocksContext.SaveChanges();
        }

        private void AssertCompanyData(Stock stock, string code, string companyName, string industryGroup)
        {
            Assert.AreEqual(code, stock.Code);
            Assert.AreEqual(companyName, stock.CompanyName);
            Assert.AreEqual(industryGroup, stock.IndustryGroup);
        }
    }
}
