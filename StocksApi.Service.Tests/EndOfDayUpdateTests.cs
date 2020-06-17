using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using StocksApi.Data;
using StocksApi.Model;
using StocksApi.Service.EndOfDayData;

namespace StocksApi.Service.Tests
{
    [TestClass]
    public class EndOfDayUpdateTests : BaseTest<EndOfDayUpdate>
    {
        private IEndOfDayStore _endOfDayStore;
        private StocksContext _stocksContext;
        private EndOfDayUpdate _endOfDayUpdate;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _endOfDayStore = Substitute.For<IEndOfDayStore>();
            _stocksContext = Substitute.For<StocksContext>();
            
            _endOfDayUpdate = new EndOfDayUpdate(
                Logger,
                _stocksContext,
                _endOfDayStore);
        }

        [TestMethod]
        public async Task Should_Update_EndOfDay_Records_From_Store()
        {
            // Arrange
            var stocks = new List<Stock>
            {
                new Stock
                {
                    Id = Guid.NewGuid(),
                    Code = "AAA"
                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    Code = "BBB"
                }
            };

            var dbSetStocks = Substitute.For<DbSet<Stock>, IQueryable<Stock>>()
                .Initialize(stocks)
                .WithAdd(stocks, _stocksContext);

            _stocksContext.Stock = dbSetStocks;

            var endOfDayEntities = new List<EndOfDay>
            {
                new EndOfDay
                {
                    Id = Guid.NewGuid(),
                    Date = new DateTime(2020, 2, 3),
                    Stock = stocks.First(),
                    Open = 0.1m,
                    High = 0.2m,
                    Low = 0.05m,
                    Close = 0.15m,
                    Volume = 1111
                },
                new EndOfDay
                {
                    Id = Guid.NewGuid(),
                    Date = new DateTime(2020, 2, 4),
                    Stock = stocks.Skip(1).First(),
                    Open = 0.01m,
                    High = 0.02m,
                    Low = 0.005m,
                    Close = 0.015m,
                    Volume = 2222
                }
            };

            var dbSetEndOfDays = Substitute.For<DbSet<EndOfDay>, IQueryable<EndOfDay>>()
                .Initialize(endOfDayEntities)
                .WithAdd(endOfDayEntities, _stocksContext)
                .WithRemove(endOfDayEntities, _stocksContext);

            _stocksContext.EndOfDay = dbSetEndOfDays;

            var endOfDays = new List<string>
            {
                "111,20200203,3.28,3.29,3.15,3.17,96249",
                "AAA,20200203,0.175,0.175,0.17,0.17,135850",
                "BBB,20200203,0.105,0.105,0.1,0.1,100000",
                "CCC,20200203,0.042,0.042,0.038,0.038,667017",
                "CCC,20200204,0.1,0.2,0.004,0.15,999999999"
            };

            _endOfDayStore
                .GetFromStore()
                .Returns(endOfDays);

            // Act
            await _endOfDayUpdate.Update();

            // Assert
            Assert.AreEqual(4, stocks.Count);

            Assert.IsTrue(stocks.Any(s => s.Code == "111"));
            Assert.IsTrue(stocks.Any(s => s.Code == "AAA"));
            Assert.IsTrue(stocks.Any(s => s.Code == "BBB"));
            Assert.IsTrue(stocks.Any(s => s.Code == "CCC"));

            Assert.AreEqual(6, endOfDayEntities.Count);

            var endOfDay1 = endOfDayEntities.Single(s => s.Stock.Code == "111");
            AssertEndOfDay(endOfDay1, 3.28m, 3.29m, 3.15m, 3.17m, 96249);

            // Old one will be removed and replaced with the new value
            var endOfDay2 = endOfDayEntities.Single(s => s.Stock.Code == "AAA");
            AssertEndOfDay(endOfDay2, 0.175m, 0.175m, 0.17m, 0.17m, 135850);

            // This one is not deleted as it was for a different day
            var endOfDay3 = endOfDayEntities.Single(s => s.Stock.Code == "BBB" && s.Date == new DateTime(2020, 2, 4));
            AssertEndOfDay(endOfDay3, 0.01m, 0.02m, 0.005m, 0.015m, 2222);

            var endOfDay4 = endOfDayEntities.Single(s => s.Stock.Code == "BBB" && s.Date == new DateTime(2020, 2, 3));
            AssertEndOfDay(endOfDay4, 0.105m, 0.105m, 0.1m, 0.1m, 100000);

            var endOfDay5 = endOfDayEntities.Single(s => s.Stock.Code == "CCC" && s.Date == new DateTime(2020, 2, 3));
            AssertEndOfDay(endOfDay5, 0.042m, 0.042m, 0.038m, 0.038m, 667017);

            var endOfDay6 = endOfDayEntities.Single(s => s.Stock.Code == "CCC" && s.Date == new DateTime(2020, 2, 3));
            AssertEndOfDay(endOfDay6, 0.1m, 0.2m, 0.004m, 0.15m, 999999999);
        }

        private void AssertEndOfDay(EndOfDay endOfDay, decimal open, decimal high, decimal low, decimal close, long volume)
        {
            Assert.AreEqual(open, endOfDay.Open);
            Assert.AreEqual(high, endOfDay.High);
            Assert.AreEqual(low, endOfDay.Low);
            Assert.AreEqual(close, endOfDay.Close);
            Assert.AreEqual(volume, endOfDay.Volume);
        }
    }
}
