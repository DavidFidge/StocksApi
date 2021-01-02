using System;

namespace StocksApi.Model
{
    public class Dividend : Entity
    {
        public Portfolio Portfolio { get; set; }
        public Stock Stock { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal DividendAmount { get; set; }
        public decimal FrankedAmount { get; set; }
        public bool IsDividendReinvestmentPlan { get; set; }
        public long ReinvestmentNumberOfShares { get; set; }
        public decimal ReinvestmentPrice { get; set; }
    }
}
