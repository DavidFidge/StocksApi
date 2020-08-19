using System;
using System.Collections.Generic;

namespace StocksApi.Model
{
    public class Portfolio : Entity
    {
        public PortfolioManager PortfolioManager { get; set; }
        public string Name { get; set; }
        public List<Holding> Holdings { get; set; }

        public Portfolio()
        {
            Holdings = new List<Holding>();
        }
    }
}
