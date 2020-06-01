using System;

namespace StocksApi
{
    public class Stock
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
