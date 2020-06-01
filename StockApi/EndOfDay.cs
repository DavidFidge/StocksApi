using System;

namespace StockApi
{
    public class EndOfDay
    {
        public Stock Stock { get; set; }
        public DateTime Date { get; set; }
        public Decimal Open { get; set; }
        public Decimal Low { get; set; }
        public Decimal High { get; set; }
        public Decimal Close { get; set; }
        public Int64 Volume { get; set; }
    }
}
