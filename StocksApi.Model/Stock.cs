using System;
using System.Collections.Generic;

namespace StocksApi.Model
{
    public class Stock : Entity
    {
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string IndustryGroup { get; set; }

        public static IEqualityComparer<Stock> EqualityComparer => new StockEqualityComparer();

        public static Stock CreateDefault(string code)
        {
            return new Stock
            {
                CompanyName = code,
                Code = code
            };
        }
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
