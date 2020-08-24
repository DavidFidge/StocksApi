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
    public class PortfolioController : BaseController<StocksContext, PortfolioDto, SavePortfolioDto, Portfolio>
    {
        public PortfolioController(StocksContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        [HttpGet("Portfolios")]
        public IQueryable<PortfolioDto> GetPortfolios()
        {
            var portfolioManager = _dbContext.PortfolioManager
                .Single();

            return _dbContext.Portfolio
                .Where(p => p.PortfolioManager == portfolioManager)
                .ProjectTo<PortfolioDto>(_mapper.ConfigurationProvider);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioDto>> GetPortfolio(Guid id)
        {
            return await GetById(_dbContext.Portfolio, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortfolio(Guid id, SavePortfolioDto savePortfolioDto)
        {
            return await PutById(id, _dbContext.Portfolio, savePortfolioDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostPortfolio(SavePortfolioDto savePortfolioDto)
        {
            var portfolioManager = _dbContext.PortfolioManager
                .Include(p => p.Portfolios)
                .Single();

            var portfolio = await PostById(_dbContext.Portfolio, savePortfolioDto);

            if (!portfolioManager.Portfolios.Contains(portfolio))
                portfolioManager.Portfolios.Add(portfolio);

            await _dbContext.SaveChangesAsync();

            return GetCreatedAtAction(nameof(GetPortfolio), portfolio);
        }

        [HttpPost("PortfolioManager")]
        public async Task<ActionResult<PortfolioManager>> PostPortfolioManager()
        {
            if (_dbContext.PortfolioManager.Any())
                return Ok();

            var portfolioManager = new PortfolioManager();

            await _dbContext.PortfolioManager.AddAsync(portfolioManager);
            await _dbContext.SaveChangesAsync();

            return new ActionResult<PortfolioManager>(portfolioManager);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(Guid id)
        {
            var portfolio = await _dbContext.Portfolio.SingleOrDefaultAsync(e => e.Id == id);

            _dbContext.Holding.RemoveRange(portfolio.Holdings);

            return await DeleteById(_dbContext.Portfolio, id);
        }
    }
}
