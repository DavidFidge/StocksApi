using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using StocksApi.Data;

namespace StocksApi.Service.Companies
{
    public interface ICompanyInformation
    {
        Task Update(StocksContext stocksContext);
    }

    public class CompanyInformation : BaseService<CompanyInformation>, ICompanyInformation
    {
        private readonly ICompanyInformationStore _companyInformationStore;

        public CompanyInformation(
            ILogger<CompanyInformation> logger,
            ICompanyInformationStore companyInformationStore)
        : base(logger)
        {
            _companyInformationStore = companyInformationStore;
        }

        public async Task Update(StocksContext stocksContext)
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

                    var stock = stocksContext.Stock.FirstOrDefault(s => s.Code == matches[1].Groups[2].Value);

                    if (stock == null)
                    {
                        stock = new Model.Stock();
                        stocksContext.Add(stock);
                    }

                    stock.CompanyName = matches[0].Groups[2].Value;
                    stock.Code = matches[1].Groups[2].Value;
                    stock.IndustryGroup = matches[2].Groups[2].Value;
                }
            }

            stocksContext.SaveChanges();
        }
    }
}
