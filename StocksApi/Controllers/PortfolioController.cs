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

        [HttpGet("{portfolioManagerId}")]
        public IQueryable<PortfolioManager> GetPortfolios(Guid portfolioManagerId)
        {
            return _dbContext.PortfolioManagers
                .Where(p => p.Id == portfolioManagerId)
                .Include(h => h.Portfolios)
                .ThenInclude(p => p.Holdings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(Guid id)
        {
            return await GetById(_dbContext.Portfolios, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortfolio(Guid id, SavePortfolioDto savePortfolioDto)
        {
            return await PutById(id, _dbContext.Portfolios, savePortfolioDto);
        }

        [HttpPost]
        public async Task<ActionResult<Portfolio>> PostPortfolio(SavePortfolioDto savePortfolioDto)
        {
            var portfolioManager = _dbContext.PortfolioManagers
                .SingleOrDefault(pm => pm.Id == savePortfolioDto.PortfolioManagerId);

            if (portfolioManager == null)
                return NotFound();

            var result = await PostById(_dbContext.Portfolios, savePortfolioDto, nameof(GetPortfolio));

            if (!portfolioManager.Portfolios.Contains(result.Value))
                portfolioManager.Portfolios.Add(result.Value);

            await _dbContext.SaveChangesAsync();

            return result;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(Guid id)
        {
            var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(e => e.Id == id);

            _dbContext.Holdings.RemoveRange(portfolio.Holdings);

            return await DeleteById(_dbContext.Portfolios, id);
        }
    }
}
