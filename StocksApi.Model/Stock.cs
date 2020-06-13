using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksApi.Model
{
    public class Stock
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string IndustryGroup { get; set; }

        public static IEqualityComparer<Stock> EqualityComparer => new StockEqualityComparer();
    }

    public class StockEqualityComparer : IEqualityComparer<Stock>
    {
        public bool Equals(Stock x, Stock y)
        {
            if (x == null && y == null)
                return true;

            if (x == null)
                return false;

            if (y == null)
                return false;

            return x.Code.Equals(y.Code);
        }

        public int GetHashCode(Stock obj)
        {
            return obj.Code.GetHashCode();
        }
    }
}
