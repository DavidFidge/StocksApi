using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class EndOfDayController : ControllerBase
    {
        private readonly StocksContext _context;
        private readonly IEndOfDayUpdate _endOfDayUpdate;

        public EndOfDayController(StocksContext context, IEndOfDayUpdate endOfDayUpdate)
        {
            _context = context;
            _endOfDayUpdate = endOfDayUpdate;
        }

        // GET: api/EndOfDayController
        [HttpGet]
        [EnableQuery(PageSize = 3655)]
        [ODataRoute]
        public IQueryable<EndOfDay> GetEndOfDay()
        {
            return _context.EndOfDay.Include(e => e.Stock);
        }

        // GET: api/EndOfDayController/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EndOfDay>> GetEndOfDay(Guid id)
        {
            var endOfDay = await _context.EndOfDay.FindAsync(id);

            if (endOfDay == null)
            {
                return NotFound();
            }

            return endOfDay;
        }

        // PUT: api/EndOfDayController/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndOfDay(Guid id, EndOfDay endOfDay)
        {
            if (id != endOfDay.Id)
            {
                return BadRequest();
            }

            _context.Entry(endOfDay).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EndOfDayExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EndOfDayController
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<EndOfDay>> PostEndOfDay(EndOfDay endOfDay)
        {
            _context.EndOfDay.Add(endOfDay);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEndOfDay", new { id = endOfDay.Id }, endOfDay);
        }

        // DELETE: api/EndOfDayController/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EndOfDay>> DeleteEndOfDay(Guid id)
        {
            var endOfDay = await _context.EndOfDay.FindAsync(id);
            if (endOfDay == null)
            {
                return NotFound();
            }

            _context.EndOfDay.Remove(endOfDay);
            await _context.SaveChangesAsync();

            return endOfDay;
        }

        [HttpPost("Update")]
        public async Task<ActionResult<Stock>> Update()
        {
            await _endOfDayUpdate.Update(_context);

            return NoContent();
        }

        private bool EndOfDayExists(Guid id)
        {
            return _context.EndOfDay.Any(e => e.Id == id);
        }
    }
}
