using System;
using System.ComponentModel.DataAnnotations;

namespace StocksApi.Controllers
{
    public class SaveHoldingDto : BaseDto
    {
        [Required]
        public string StockCode { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public long NumberOfShares { get; set; }

        [Required]
        public Decimal PurchasePrice { get; set; }

        [Required]
        public Decimal Brokerage { get; set; }

        public Guid PortfolioId { get; set; }
    }
}