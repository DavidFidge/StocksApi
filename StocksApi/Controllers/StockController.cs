using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using StocksApi.Models;

namespace StocksApi.Controllers
{
    public class StockController : BaseController
    {
        private readonly IRepository _repository;

        public StockController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<Stock>> Get()
        {
            return await _repository.GetStocks();
        }
    }
}
