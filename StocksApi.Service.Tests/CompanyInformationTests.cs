using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using StocksApi.Data;
using StocksApi.Model;
using StocksApi.Service.Companies;

namespace StocksApi.Service.Tests
{
    [TestClass]
    public class CompanyInformationTests : BaseTest<CompanyInformation>
    {
        private ICompanyInformationStore _companyInformationStore;
        private StocksContext _stocksContext;
        private CompanyInformation _companyInformation;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _companyInformationStore = Substitute.For<ICompanyInformationStore>();
            _stocksContext = Substitute.For<StocksContext>();
            
            _companyInformation = new CompanyInformation(
                Logger,
                _stocksContext,
                _companyInformationStore);
        }

        [TestMethod]
        public async Task Update_Should_Add_New_Stocks()
        {
            // Arrange
            var stocks = new List<Stock>();

            var dbSet = Substitute.For<DbSet<Stock>, IQueryable<Stock>>()
                .Initialize(stocks)
                .WithAddRemove(stocks, _stocksContext);

            _stocksContext.Stock = dbSet;

            var asxData =
                "ASX listed companies as at Sat Jun 13 20:40:46 AEST 2020\r\n\r\nCompany name,ASX code,GICS industry group\r\n\"MOQ LIMITED\",\"MOQ\",\"Software & Services\"\r\n\"1300 SMILES LIMITED\",\"ONT\",\"Health Care Equipment & Services\"\r\n\"1414 DEGREES LIMITED\",\"14D\",\"Capital Goods\"\r\n";

            _companyInformationStore
                .GetFromStore()
                .Returns(asxData);

            // Act
            await _companyInformation.Update();

            // Assert
            Assert.AreEqual(3, stocks.Count);

            AssertCompanyData(stocks[0], "MOQ", "MOQ LIMITED", "Software & Services");
            AssertCompanyData(stocks[1], "ONT", "1300 SMILES LIMITED", "Health Care Equipment & Services");
            AssertCompanyData(stocks[2], "14D", "1414 DEGREES LIMITED", "Capital Goods");
        }

        [TestMethod]
        public async Task Update_Should_Update_Existing_Stocks()
        {
            // Arrange
            var stocks = new List<Stock>
            {
                new Stock
                {
                    Code = "MOQ",
                    CompanyName = "Old Company Name",
                    IndustryGroup = "Old Industry Group"
                }
            };

            var dbSet = Substitute.For<DbSet<Stock>, IQueryable<Stock>>()
                .Initialize(stocks)
                .WithAddRemove(stocks, _stocksContext);

            _stocksContext.Stock = dbSet;

            var asxData =
                "ASX listed companies as at Sat Jun 13 20:40:46 AEST 2020\r\n\r\nCompany name,ASX code,GICS industry group\r\n\"New Company Name\",\"MOQ\",\"New Industry Group\"\r\n";

            _companyInformationStore
                .GetFromStore()
                .Returns(asxData);

            // Act
            await _companyInformation.Update();

            // Assert
            Assert.AreEqual(1, stocks.Count);
            AssertCompanyData(stocks[0], "MOQ", "New Company Name", "New Industry Group");
        }

        private void AssertCompanyData(Stock stock, string code, string companyName, string industryGroup)
        {
            Assert.AreEqual(code, stock.Code);
            Assert.AreEqual(companyName, stock.CompanyName);
            Assert.AreEqual(industryGroup, stock.IndustryGroup);
        }
    }
}
