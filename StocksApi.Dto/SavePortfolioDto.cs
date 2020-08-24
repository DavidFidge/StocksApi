using System;

namespace StocksApi.Controllers
{
    public class SavePortfolioDto : BaseDto
    {
        public string Name { get; set; }
        public string HolderIdentificationNumber { get; set; }
    }
}