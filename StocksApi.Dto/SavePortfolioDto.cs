using System;

namespace StocksApi.Controllers
{
    public class SavePortfolioDto : BaseSaveDto
    {
        public Guid PortfolioManagerId { get; set; }
        public string Name { get; set; }
    }
}