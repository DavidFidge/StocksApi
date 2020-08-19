using System;

namespace StocksApi.Controllers
{
    public class StockDto : BaseDto
    {
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string IndustryGroup { get; set; }
    }
}