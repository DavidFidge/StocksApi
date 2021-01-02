using System;

namespace StocksApi.Controllers
{
    public class DividendDto : BaseDto
    {
        public string StockCode { get; set; }
        public Guid PortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal DividendAmount { get; set; }
        public decimal FrankedAmount { get; set; }
        public bool IsDividendReinvestmentPlan { get; set; }
        public long ReinvestmentNumberOfShares { get; set; }
        public decimal ReinvestmentPrice { get; set; }
    }
}