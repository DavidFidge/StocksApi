using System;

namespace StocksApi.Controllers
{
    public class SaveHoldingDto : BaseSaveDto
    {
        public string StockCode { get; set; }
        public DateTime PurchaseDate { get; set; }
        public long NumberOfShares { get; set; }
        public Decimal PurchasePrice { get; set; }
        public Decimal Brokerage { get; set; }
        public Guid PortfolioId { get; set; }
    }
}