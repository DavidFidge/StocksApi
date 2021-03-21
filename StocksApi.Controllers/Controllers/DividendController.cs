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
    public class DividendController : BaseController<StocksContext, DividendDto, SaveDividendDto, Dividend>
    {
        public DividendController(StocksContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        [HttpGet("Dividends")]
        public IQueryable<DividendDto> GetDividends()
        {
            var portfolioManager = _dbContext.PortfolioManager
                .Single();

            return _dbContext.Portfolio
                .Include(p => p.Dividends)
                .Where(p => p.PortfolioManager == portfolioManager)
                .SelectMany(p => p.Dividends)
                .ProjectTo<DividendDto>(_mapper.ConfigurationProvider);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DividendDto>> GetDividend(Guid id)
        {
            return await GetById(_dbContext.Dividend, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDividend(Guid id, SaveDividendDto saveDividendDto)
        {
            return await PutById(id, _dbContext.Dividend, saveDividendDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostDividend(SaveDividendDto saveDividendDto)
        {
            var portfolio = _dbContext.Portfolio
                .Single(p => p.Id == saveDividendDto.PortfolioId);

            var dividend = await PostById(_dbContext.Dividend, saveDividendDto);
            
            dividend.Stock = _dbContext.Stock.Single(s => s.Code == saveDividendDto.StockCode);

            if (!portfolio.Dividends.Contains(dividend))
                portfolio.Dividends.Add(dividend);

            await _dbContext.SaveChangesAsync();

            return GetCreatedAtAction(nameof(GetDividend), dividend);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDividend(Guid id)
        {
            return await DeleteById(_dbContext.Dividend, id);
        }
    }
}
