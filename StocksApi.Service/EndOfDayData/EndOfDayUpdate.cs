using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using StocksApi.Data;
using StocksApi.Model;

namespace StocksApi.Service.EndOfDayData
{
    public interface IEndOfDayUpdate
    {
        Task Update();
    }

    public class EndOfDayUpdate : BaseService<EndOfDayUpdate>, IEndOfDayUpdate
    {
        private readonly StocksContext _stockContext;

        public EndOfDayUpdate(
            ILogger<EndOfDayUpdate> logger,
            StocksContext stockContext)
        : base(logger)
        {
            _stockContext = stockContext;
        }

        public async Task Update()
        {
            var files = Directory.EnumerateFiles("C:\\dev\\StocksApi\\StocksApi\\csv", "*.txt");

            var endOfDays = new List<EndOfDay>();

            var allStocks = _stockContext
                .Stock.ToHashSet();

            foreach (var file in files)
                await GetEndOfDaysForFile(file, allStocks, endOfDays);

            DeleteExistingEndOfDaysForSameDates(endOfDays);

            _stockContext.AddRange(endOfDays);

            _stockContext.SaveChanges();
        }

        private async Task GetEndOfDaysForFile(
            string file,
            HashSet<Stock> allStocks,
            List<EndOfDay> endOfDays)
        {
            var endOfDayLines = await File.ReadAllLinesAsync(file);

            foreach (var endOfDayLine in endOfDayLines)
            {
                var endOfDaySplit = endOfDayLine.Split(",");

                var stockCode = endOfDaySplit[0];

                var stock = GetOrAddStock(allStocks, stockCode);

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
        }

        private Stock GetOrAddStock(HashSet<Stock> allStocks, string stockCode)
        {
            var stock = allStocks.FirstOrDefault(s => s.Code == stockCode);

            if (stock != null)
                return stock;

            stock = new Stock
            {
                Code = stockCode,
                CompanyName = stockCode
            };

            _stockContext.Add(stock);
            allStocks.Add(stock);

            return stock;
        }

        private void DeleteExistingEndOfDaysForSameDates(List<EndOfDay> endOfDays)
        {
            var endOfDayDates = endOfDays
                .Select(e => e.Date)
                .Distinct()
                .ToList();

            var endOfDaysToDelete = _stockContext.EndOfDay
                .Where(e => endOfDayDates.Contains(e.Date));

            _stockContext.RemoveRange(endOfDaysToDelete);

            _stockContext.SaveChanges();
        }
    }
}
