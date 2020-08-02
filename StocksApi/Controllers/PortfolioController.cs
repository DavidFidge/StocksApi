using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksApi.Data;
using StocksApi.Model;

namespace StocksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : BaseController<StocksContext, SavePortfolioDto, Portfolio>
    {
        public PortfolioController(StocksContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        [HttpGet("Portfolios")]
        public IQueryable<PortfolioManager> GetPortfolios()
        {
            var portfolioManager = _dbContext.PortfolioManager
                .Single();

            return _dbContext.PortfolioManager
                .Where(p => p.Id == portfolioManager.Id)
                .Include(h => h.Portfolios)
                .ThenInclude(p => p.Holdings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(Guid id)
        {
            return await GetById(_dbContext.Portfolio, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortfolio(Guid id, SavePortfolioDto savePortfolioDto)
        {
            return await PutById(id, _dbContext.Portfolio, savePortfolioDto);
        }

        [HttpPost]
        public async Task<ActionResult<Portfolio>> PostPortfolio(SavePortfolioDto savePortfolioDto)
        {
            var portfolioManager = _dbContext.PortfolioManager
                .Include(p => p.Portfolios)
                .Single();

            var result = await PostById(_dbContext.Portfolio, savePortfolioDto, nameof(GetPortfolio));

            var createdAtActionResult = result.Result as CreatedAtActionResult;

            createdAtActionResult.Value as Portfolio;

            if (!portfolioManager.Portfolios.Contains(result.Value))
                portfolioManager.Portfolios.Add(result.Value);

            await _dbContext.SaveChangesAsync();

            return result;
        }

        [HttpPost("PortfolioManager")]
        public async Task<ActionResult<PortfolioManager>> PostPortfolioManager()
        {
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
