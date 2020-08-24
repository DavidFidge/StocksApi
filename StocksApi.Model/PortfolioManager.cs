using System;
using System.Collections.Generic;

namespace StocksApi.Model
{
    public class PortfolioManager : Entity
    {
        public string Name { get; set; }
        public List<Portfolio> Portfolios { get; set; }

        public PortfolioManager()
        {
            Portfolios = new List<Portfolio>();
        }
    }
}
