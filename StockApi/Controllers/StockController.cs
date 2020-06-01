using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;

        public StockController(ILogger<StockController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<EndOfDay>> Get()
        {
            return await GetStockEndOfDays();
        }

        public async Task<List<EndOfDay>> GetStockEndOfDays()
        {
            var files = Directory.EnumerateFiles("C:\\dev\\StockApi\\StockApi\\csv");

            var endOfDays = new List<EndOfDay>();
            var stocks = new Dictionary<string, Stock>();

            foreach (var file in files)
            {
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
                                Code = stockCode, Description = stockCode

                            });
                    }

                    endOfDays.Add(
                        new EndOfDay
                        {
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

            return endOfDays;
        }
    }
}
