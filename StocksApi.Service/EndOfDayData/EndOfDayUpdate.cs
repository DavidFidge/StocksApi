using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using StocksApi.Data;
using StocksApi.Model;

namespace StocksApi.Service.EndOfDayData
{
    public interface IEndOfDayUpdate
    {
        Task Update(StocksContext stocksContext);
    }

    public class EndOfDayUpdate : BaseService<EndOfDayUpdate>, IEndOfDayUpdate
    {
        private readonly IEndOfDayStore _endOfDayStore;

        public EndOfDayUpdate(
            ILogger<EndOfDayUpdate> logger,
            IEndOfDayStore endOfDayStore)
        : base(logger)
        {
            _endOfDayStore = endOfDayStore;
        }

        public async Task Update(StocksContext stocksContext)
        {
            var endOfDays = await GetEndOfDays(stocksContext);

            DeleteExistingEndOfDaysForSameDates(endOfDays, stocksContext);

            stocksContext.EndOfDay.AddRange(endOfDays);
            stocksContext.SaveChanges();
        }

        private void DeleteExistingEndOfDaysForSameDates(IList<EndOfDay> endOfDays, StocksContext stocksContext)
        {
            var endOfDayDates = endOfDays
                .Select(e => e.Date)
                .Distinct()
                .ToList();

            var endOfDaysToDelete = stocksContext.EndOfDay
                .Where(e => endOfDayDates.Contains(e.Date))
                .ToList();

            stocksContext.EndOfDay.RemoveRange(endOfDaysToDelete);
            stocksContext.SaveChanges();
        }

        private async Task<IList<EndOfDay>> GetEndOfDays(StocksContext stocksContext)
        {
            var endOfDays = new List<EndOfDay>();

            var allStocks = stocksContext
                .Stock
                .ToList();

            var allStocksWorking = allStocks
                .ToHashSet(Stock.EqualityComparer);

            var endOfDayLines = await _endOfDayStore.GetFromStore();

            foreach (var endOfDayLine in endOfDayLines)
            {
                var endOfDaySplit = endOfDayLine.Split(",");

                var stockCode = endOfDaySplit[0];

                var stock = GetOrAddStock(allStocksWorking, stockCode);

                endOfDays.Add(
                    new EndOfDay
                    {
                        Stock = stock,
                        Date = DateTime.ParseExact(endOfDaySplit[1], "yyyyMMdd", CultureInfo.InvariantCulture),
                        Open = Decimal.Parse(endOfDaySplit[2]),
                        High = Decimal.Parse(endOfDaySplit[3]),
                        Low = Decimal.Parse(endOfDaySplit[4]),
                        Close = Decimal.Parse(endOfDaySplit[5]),
                        Volume = Int64.Parse(endOfDaySplit[6])
                    });
            }

            var newStocks = allStocksWorking
                .Except(allStocks, Stock.EqualityComparer)
                .ToList();

            stocksContext.Stock.AddRange(newStocks);

            return endOfDays;
        }

        private Stock GetOrAddStock(HashSet<Stock> allStocks, string stockCode)
        {
            var stock = allStocks.FirstOrDefault(s => s.Code == stockCode);

            if (stock != null)
                return stock;

            stock = Stock.CreateDefault(stockCode);

            allStocks.Add(stock);

            return stock;
        }
    }
}
