using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StocksApi.Models;

namespace StocksApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly IRepository _repository;

        public StockController(ILogger<StockController> logger, IRepository repository)
        {
            _logger = logger;
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
