using System;

namespace StocksApi.Controllers
{
    public class SavePortfolioDto : BaseSaveDto
    {
        public string Name { get; set; }
    }
}