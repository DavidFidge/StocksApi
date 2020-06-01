using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StocksApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EndOfDayController : ControllerBase
    {
        private readonly ILogger<EndOfDayController> _logger;
        private static IList<EndOfDay> _endOfDayCache;

        public EndOfDayController(ILogger<EndOfDayController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IEnumerable<EndOfDay>> Get()
        {
            if (_endOfDayCache == null)
                await GetStockEndOfDays();

            return _endOfDayCache;
        }

        private async Task GetStockEndOfDays()
        {
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
                                Description = stockCode
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
        }
    }
}
