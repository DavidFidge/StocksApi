using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using StocksApi.Models;

namespace StocksApi.Controllers
{
    public class EndOfDayController : BaseController
    {
        private readonly IRepository _repository;

        public EndOfDayController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<EndOfDay>> Get()
        {
            return await _repository.GetEndOfDay();
        }
    }
}