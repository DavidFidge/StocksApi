using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksApi.Data;
using StocksApi.Model;

namespace StocksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoldingController : BaseController<StocksContext, HoldingDto, SaveHoldingDto, Holding>
    {
        public HoldingController(StocksContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        [HttpGet("Holdings")]
        public IQueryable<HoldingDto> GetHoldings()
        {
            var portfolioManager = _dbContext.PortfolioManager
                .Single();

            return _dbContext.Portfolio
                .Include(p => p.Holdings)
                .Where(p => p.PortfolioManager == portfolioManager)
                .SelectMany(p => p.Holdings)
                .ProjectTo<HoldingDto>(_mapper.ConfigurationProvider);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HoldingDto>> GetHolding(Guid id)
        {
            return await GetById(_dbContext.Holding, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHolding(Guid id, SaveHoldingDto saveHoldingDto)
        {
            return await PutById(id, _dbContext.Holding, saveHoldingDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostHolding(SaveHoldingDto saveHoldingDto)
        {
            var portfolio = _dbContext.Portfolio
                .Single(p => p.Id == saveHoldingDto.PortfolioId);

            var holding = await PostById(_dbContext.Holding, saveHoldingDto);
            
            holding.Stock = _dbContext.Stock.Single(s => s.Code == saveHoldingDto.StockCode);;

            if (!portfolio.Holdings.Contains(holding))
                portfolio.Holdings.Add(holding);

            await _dbContext.SaveChangesAsync();

            return GetCreatedAtAction(nameof(GetHolding), holding);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHolding(Guid id)
        {
            return await DeleteById(_dbContext.Holding, id);
        }
    }
}
