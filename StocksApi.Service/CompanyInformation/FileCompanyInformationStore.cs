﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StocksApi.Service.Stock
{
    public class FileCompanyInformationStore : BaseService<FileCompanyInformationStore>, ICompanyInformationStore
    {
        private readonly ILogger<FileCompanyInformationStore> _logger;

        public FileCompanyInformationStore(ILogger<FileCompanyInformationStore> logger)
            : base(logger)
        {
            _logger = logger;
        }

        public async Task<string> GetFromStore()
        {
            var content =
                await File.ReadAllTextAsync(Path.Join(Directory.GetCurrentDirectory(), "ASXListedCompanies.csv"));

            return content;
        }
    }
}
