using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using StocksApi.Data;
using StocksApi.Model;
using StocksApi.Service.EndOfDayData;

namespace StocksApi.Service.Tests
{
    [TestClass]
    public class EndOfDayUpdateTests : BaseSqlLiteTest<EndOfDayUpdate, StocksContext>
    {
        private IEndOfDayStore _endOfDayStore;
        private EndOfDayUpdate _endOfDayUpdate;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            using var stocksContext = new StocksContext(ContextOptions);

            SetupDatabase(stocksContext);

            _endOfDayStore = Substitute.For<IEndOfDayStore>();
            
            _endOfDayUpdate = new EndOfDayUpdate(Logger, _endOfDayStore);
        }

        [TestMethod]
        public async Task Update_Should_Add_New_EndOfDay_Record()
        {
            // Arrange
            var arrangeStocks = new List<Stock>
            {
                new Stock
                {
                    Code = "111"
                }
            };

            using (var arrangeStocksContext = new StocksContext(ContextOptions))
            {
                arrangeStocksContext.Stock.AddRange(arrangeStocks);
                arrangeStocksContext.SaveChanges();
            }

            var endOfDays = new List<string>
            {
                "111,20200203,3.28,3.29,3.15,3.17,96249"
            };

            _endOfDayStore
                .GetFromStore()
                .Returns(endOfDays);

            // Act
            using (var actStocksContext = new StocksContext(ContextOptions))
            {
                await _endOfDayUpdate.Update(actStocksContext);
            }
            
            // Assert
            using var stocksContext = new StocksContext(ContextOptions);

            var endOfDayEntities = stocksContext.EndOfDay.ToList();

            var stocks = stocksContext.Stock.ToList();


            Assert.AreEqual(1, endOfDayEntities.Count);

            Assert.AreEqual(stocks[0], endOfDayEntities[0].Stock);
            AssertEndOfDay(endOfDayEntities[0], new DateTime(2020, 2, 3), 3.28m, 3.29m, 3.15m, 3.17m, 96249);
        }

        [TestMethod]
        public async Task Update_Should_Add_New_EndOfDay_Records_With_New_Placeholder_Stock_If_Not_Exists()
        {
            // Arrange
            var endOfDays = new List<string>
            {
                "111,20200203,3.28,3.29,3.15,3.17,96249"
            };

            _endOfDayStore
                .GetFromStore()
                .Returns(endOfDays);

            // Act
            using (var actStocksContext = new StocksContext(ContextOptions))
            {
                await _endOfDayUpdate.Update(actStocksContext);
            }

            // Assert
            using var stocksContext = new StocksContext(ContextOptions);

            var endOfDayEntities = stocksContext.EndOfDay.ToList();

            var stocks = stocksContext.Stock.ToList();

            Assert.AreEqual(1, stocks.Count);
            var stock = stocks[0];

            Assert.AreEqual("111", stock.Code);
            Assert.AreEqual("111", stock.CompanyName);
            Assert.IsNull(stock.IndustryGroup);

            Assert.AreEqual(1, endOfDayEntities.Count);

            Assert.AreEqual(stock, endOfDayEntities[0].Stock);
            AssertEndOfDay(endOfDayEntities[0], new DateTime(2020, 2, 3), 3.28m, 3.29m, 3.15m, 3.17m, 96249);
        }

        [TestMethod]
        public async Task Update_Should_Add_New_EndOfDay_Record_For_Multiple_Days()
        {
            // Arrange
            var arrangeStocks = new List<Stock>
            {
                new Stock
                {
                    Code = "111"
                }
            };

            using (var arrangeStocksContext = new StocksContext(ContextOptions))
            {
                arrangeStocksContext.Stock.AddRange(arrangeStocks);
                arrangeStocksContext.SaveChanges();
            }

            var endOfDays = new List<string>
            {
                "111,20200203,3.28,3.29,3.15,3.17,96249",
                "111,20200204,4.28,4.29,4.15,4.17,46249",
            };

            _endOfDayStore
                .GetFromStore()
                .Returns(endOfDays);

            // Act
            using (var actStocksContext = new StocksContext(ContextOptions))
            {
                await _endOfDayUpdate.Update(actStocksContext);
            }

            // Assert
            using var stocksContext = new StocksContext(ContextOptions);

            var endOfDayEntities = stocksContext.EndOfDay.ToList();

            var stocks = stocksContext.Stock.ToList();

            Assert.AreEqual(2, endOfDayEntities.Count);

            Assert.AreEqual(stocks[0], endOfDayEntities[0].Stock);
            AssertEndOfDay(endOfDayEntities[0], new DateTime(2020, 2, 3), 3.28m, 3.29m, 3.15m, 3.17m, 96249);

            Assert.AreEqual(stocks[0], endOfDayEntities[0].Stock);
            AssertEndOfDay(endOfDayEntities[1], new DateTime(2020, 2, 4), 4.28m, 4.29m, 4.15m, 4.17m, 46249);
        }

        [TestMethod]
        public async Task Update_Should_Replace_EndOfDay_Records()
        {
            // Arrange
            var arrangeStocks = new List<Stock>
            {
                new Stock
                {
                    Id = Guid.NewGuid(),
                    Code = "111"
                }
            };

            var arrangeEndOfDayEntities = new List<EndOfDay>
            {
                new EndOfDay
                {
                    Id = Guid.NewGuid(),
                    Date = new DateTime(2020, 2, 3),
                    Stock = arrangeStocks.First(),
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
                    Stock = arrangeStocks.First(),
                    Open = 0.01m,
                    High = 0.02m,
                    Low = 0.005m,
                    Close = 0.015m,
                    Volume = 2222
                }
            };

            using (var arrangeStocksContext = new StocksContext(ContextOptions))
            {
                arrangeStocksContext.Stock.AddRange(arrangeStocks);
                arrangeStocksContext.EndOfDay.AddRange(arrangeEndOfDayEntities);

                arrangeStocksContext.SaveChanges();

            }

            var endOfDays = new List<string>
            {
                "111,20200203,0.175,0.175,0.17,0.17,135850"
            };

            _endOfDayStore
                .GetFromStore()
                .Returns(endOfDays);

            // Act
            using (var actStocksContext = new StocksContext(ContextOptions))
            {
                await _endOfDayUpdate.Update(actStocksContext);
            }

            // Assert
            using var stocksContext = new StocksContext(ContextOptions);

            var endOfDayEntities = stocksContext.EndOfDay.ToList();

            var stocks = stocksContext.Stock.ToList();

            Assert.AreEqual(1, stocks.Count);

            Assert.AreEqual(2, endOfDayEntities.Count);

            var endOfDayUpdated = endOfDayEntities.Single(e => e.Stock.Code == "111" && e.Date == new DateTime(2020, 2, 3));
            AssertEndOfDay(endOfDayUpdated, new DateTime(2020, 2, 3), 0.175m, 0.175m, 0.17m, 0.17m, 135850);

            var endOfDaySame = endOfDayEntities.Single(e => e.Stock.Code == "111" && e.Date == new DateTime(2020, 2, 4));
            AssertEndOfDay(endOfDaySame, new DateTime(2020, 2, 4), 0.01m, 0.02m, 0.005m, 0.015m, 2222);
        }
        
        private void AssertEndOfDay(EndOfDay endOfDay, DateTime date, decimal open, decimal high, decimal low, decimal close, long volume)
        {
            Assert.AreEqual(open, endOfDay.Open);
            Assert.AreEqual(high, endOfDay.High);
            Assert.AreEqual(low, endOfDay.Low);
            Assert.AreEqual(close, endOfDay.Close);
            Assert.AreEqual(volume, endOfDay.Volume);
            Assert.AreEqual(date, endOfDay.Date);
        }
    }
}
