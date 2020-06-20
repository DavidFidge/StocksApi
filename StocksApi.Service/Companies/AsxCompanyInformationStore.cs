using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using StocksApi.Core;

namespace StocksApi.Service.Companies
{
    public class AsxCompanyInformationStore : BaseService<AsxCompanyInformationStore>, ICompanyInformationStore
    {
        private static HttpClient _httpClient = new HttpClient();

        private readonly Uri _uri;

        public AsxCompanyInformationStore(ILogger<AsxCompanyInformationStore> logger)
            : base(logger)
        {
            _uri = new Uri(Environment.GetEnvironmentVariable(Constants.StocksApiAsxListedCompaniesUrl) ?? "https://www.asx.com.au/asx/research/ASXListedCompanies.csv");
        }

        public async Task<string> GetFromStore()
        {
            var response = await _httpClient.GetAsync(_uri);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}
