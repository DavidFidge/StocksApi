using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StocksApi.Service.EndOfDayData
{
    public class FileEndOfDayStore : BaseService<FileEndOfDayStore>, IEndOfDayStore
    {
        public FileEndOfDayStore(ILogger<FileEndOfDayStore> logger)
            : base(logger)
        {
        }

        public async Task<IList<string>> GetFromStore()
        {
            var files = Directory.EnumerateFiles("C:\\dev\\StocksApi\\StocksApi\\csv", "*.txt");

            var endOfDays = await Task.WhenAll(files.Select(async f => await File.ReadAllLinesAsync(f)));

            return endOfDays
                .SelectMany(s => s)
                .ToList();
        }
    }
}