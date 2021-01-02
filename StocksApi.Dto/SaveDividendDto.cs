using System;
using System.ComponentModel.DataAnnotations;

namespace StocksApi.Controllers
{
    public class SaveDividendDto : BaseDto
    {
        [Required]
        public string StockCode { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public decimal DividendAmount { get; set; }

        public decimal FrankedAmount { get; set; }

        public bool IsDividendReinvestmentPlan { get; set; }

        public long ReinvestmentNumberOfShares { get; set; }

        public decimal ReinvestmentPrice { get; set; }

        public Guid PortfolioId { get; set; }
    }
}