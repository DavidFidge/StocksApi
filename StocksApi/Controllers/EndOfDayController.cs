﻿using System;
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
    public class EndOfDayController : ControllerBase
    {
        private readonly ILogger<EndOfDayController> _logger;
        private readonly IRepository _repository;

        public EndOfDayController(ILogger<EndOfDayController> logger, IRepository repository)
        {
            _logger = logger;
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