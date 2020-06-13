using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using StocksApi.Model;

namespace StocksApi.Models
{
    public interface IRepository
    {
        Task<IQueryable<Stock>> GetStocks();
        Task<IQueryable<EndOfDay>> GetEndOfDay();
    }

    public class Repository : IRepository
    {
        private readonly ILogger<Repository> _logger;
        private List<EndOfDay> _endOfDayCache;
        private List<Stock> _stockCache;

        public Repository(ILogger<Repository> logger)
        {
            _logger = logger;
        }

        public async Task<IQueryable<Stock>> GetStocks()
        {
            if (_stockCache != null)
                return _stockCache.AsQueryable();

            var stocks = await GetStockEndOfDays();

            _stockCache = stocks
                .Select(s => s.Stock)
                .Distinct(Stock.EqualityComparer)
                .ToList();

            return _stockCache.AsQueryable();
        }

        public async Task<IQueryable<EndOfDay>> GetEndOfDay()
        {
            return await GetStockEndOfDays();
        }

        private async Task<IQueryable<EndOfDay>> GetStockEndOfDays()
        {
            if (_endOfDayCache != null)
                return _endOfDayCache.AsQueryable();

            var files = Directory.EnumerateFiles("C:\\dev\\StocksApi\\StocksApi\\csv", "*.txt");

            var endOfDays = new List<EndOfDay>();
            var stocks = new Dictionary<string, Stock>();

            foreach (var file in files)
            {
                _logger.LogInformation($"Processing {file}");

                var endOfDayLines = await System.IO.File.ReadAllLinesAsync(file);

                foreach (var endOfDayLine in endOfDayLines)
                {
                    var endOfDaySplit = endOfDayLine.Split(",");

                    var stockCode = endOfDaySplit[0];

                    if (!stocks.ContainsKey(stockCode))
                    {
                        stocks.Add(
                            stockCode, new Stock
                            {
                                Id = Guid.NewGuid(),
                                Code = stockCode,
                                CompanyName = stockCode
                            });
                    }

                    endOfDays.Add(
                        new EndOfDay
                        {
                            Id = Guid.NewGuid(),
                            Stock = stocks[stockCode],
                            Date = DateTime.ParseExact(endOfDaySplit[1], "yyyyMMdd", CultureInfo.InvariantCulture),
                            Open = Decimal.Parse(endOfDaySplit[2]),
                            High = Decimal.Parse(endOfDaySplit[3]),
                            Low = Decimal.Parse(endOfDaySplit[4]),
                            Close = Decimal.Parse(endOfDaySplit[5]),
                            Volume = Int64.Parse(endOfDaySplit[6])
                        });
                }
            }

            _endOfDayCache = endOfDays;

            return _endOfDayCache.AsQueryable();
        }
    }
}
