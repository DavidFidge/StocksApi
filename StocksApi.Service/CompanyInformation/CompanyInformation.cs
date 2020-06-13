using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StocksApi.Data;

namespace StocksApi.Service.Stock
{
    public interface ICompanyInformation
    {
        Task Update();
    }

    public class CompanyInformation : BaseService<CompanyInformation>, ICompanyInformation
    {
        private readonly StocksContext _context;
        private readonly ICompanyInformationStore _companyInformationStore;

        public CompanyInformation(
            ILogger<CompanyInformation> logger,
            StocksContext context,
            ICompanyInformationStore companyInformationStore)
        : base(logger)
        {
            _context = context;
            _companyInformationStore = companyInformationStore;
        }

        public async Task Update()
        {
            var content = await _companyInformationStore.GetFromStore();

            var lines = content.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            if (lines.Length > 3)
            {
                for (var i = 3; i < lines.Length; i++)
                {
                    // https://stackoverflow.com/questions/18144431/regex-to-split-a-csv
                    var matches = Regex.Matches(lines[i], @"(?:^|,)(?=[^""]|("")?)""?((?(1)[^""]*|[^,""]*))""?(?=,|$)");

                    if (matches.Count != 3)
                        continue;

                    var stock = _context.Stock.FirstOrDefault(s => s.Code == matches[1].Groups[2].Value);

                    if (stock == null)
                    {
                        stock = new Model.Stock();
                        _context.Add(stock);
                    }

                    stock.CompanyName = matches[0].Groups[2].Value;
                    stock.Code = matches[1].Groups[2].Value;
                    stock.CompanyName = matches[1].Groups[2].Value;

                    _context.SaveChanges();
                }
            }
        }
    }
}
