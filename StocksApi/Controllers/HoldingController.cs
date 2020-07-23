using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using StocksApi.Data;
using StocksApi.Model;

namespace StocksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoldingController : BaseController<StocksContext, SaveHoldingDto, Holding>
    {
        public HoldingController(StocksContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Holding>> GetHolding(Guid id)
        {
            return await GetById(_dbContext.Holdings, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHolding(Guid id, SaveHoldingDto saveHoldingDto)
        {
            return await PutById(id, _dbContext.Holdings, saveHoldingDto);
        }

        [HttpPost]
        public async Task<ActionResult<Holding>> PostHolding(SaveHoldingDto saveHoldingDto)
        {
            var portfolio = _dbContext.Portfolios
                .SingleOrDefault(p => p.Id == saveHoldingDto.PortfolioId);

            if (portfolio == null)
                return NotFound();

            var result = await PostById(_dbContext.Holdings, saveHoldingDto, nameof(GetHolding));

            if (!portfolio.Holdings.Contains(result.Value))
                portfolio.Holdings.Add(result.Value);

            await _dbContext.SaveChangesAsync();

            return result;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHolding(Guid id)
        {
            return await DeleteById(_dbContext.Holdings, id);
        }
    }
}
