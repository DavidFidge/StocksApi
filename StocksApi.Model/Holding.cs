using System;

namespace StocksApi.Model
{
    public class Holding : Entity
    {
        public Stock Stock { get; set; }
        public DateTime PurchaseDate { get; set; }
        public long NumberOfShares { get; set; }
        public Decimal PurchasePrice { get; set; }
        public Decimal Brokerage { get; set; }
    }
}
