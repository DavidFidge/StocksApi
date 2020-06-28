using System;
using System.Collections.Generic;

namespace StocksApi.Model
{
    public class Portfolio : Entity
    {
        public string Name { get; set; }
        public List<Holding> Holdings { get; set; }
    }
}
