using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksApi.Data;
using StocksApi.Model;
using StocksApi.Service.EndOfDayData;

namespace StocksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndOfDayController : BaseController<StocksContext, SaveEndOfDayDto, EndOfDay>
    {
        private readonly IEndOfDayUpdate _endOfDayUpdate;

        public EndOfDayController(StocksContext dbContext, IEndOfDayUpdate endOfDayUpdate, IMapper mapper)
            : base(dbContext, mapper)

        {
            _endOfDayUpdate = endOfDayUpdate;
        }

        [HttpGet]
        [EnableQuery(PageSize = 3655)]
        [ODataRoute]
        public IQueryable<EndOfDay> GetEndOfDays()
        {
            return _dbContext.EndOfDay.Include(e => e.Stock);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EndOfDay>> GetEndOfDay(Guid id)
        {
            return await GetById(_dbContext.EndOfDay, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndOfDay(Guid id, SaveEndOfDayDto saveEndOfDayDto)
        {
            return await PutById(id, _dbContext.EndOfDay, saveEndOfDayDto);
        }

        [HttpPost]
        public async Task<ActionResult<EndOfDay>> PostEndOfDay(SaveEndOfDayDto saveEndOfDayDto)
        {
            var result = await PostById(_dbContext.EndOfDay, saveEndOfDayDto, nameof(GetEndOfDay));

            Stock stock;

            if (saveEndOfDayDto.StockCode != null)
            {
                stock = await _dbContext.Stock.SingleOrDefaultAsync(s => s.Code == saveEndOfDayDto.StockCode);

                if (stock == null)
                {
                    stock = Stock.CreateDefault(saveEndOfDayDto.StockCode);

                    _dbContext.Stock.Add(stock);
                }
            }
            else
            {
                stock = await _dbContext.Stock.FindAsync(saveEndOfDayDto.StockId);
            }

            if (stock == null)
                return NotFound();

            result.Value.Stock = stock;

            await _dbContext.SaveChangesAsync();

            return result;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<EndOfDay>> DeleteEndOfDay(Guid id)
        {
            return await DeleteById(_dbContext.EndOfDay, id);
        }

        [HttpPost("Update")]
        public async Task<ActionResult<Stock>> Update()
        {
            await _endOfDayUpdate.Update(_dbContext);

            return NoContent();
        }
    }
}
