using System;

namespace StocksApi.Controllers
{
    public class PortfolioDto : BaseDto
    {
        public string Name { get; set; }
        public string HolderIdentificationNumber { get; set; }
    }
}