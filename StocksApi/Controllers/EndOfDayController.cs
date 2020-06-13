using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksApi.Data;
using StocksApi.Model;
using StocksApi.Models;

namespace StocksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndOfDayController : ControllerBase
    {
        private readonly StocksContext _context;

        public EndOfDayController(StocksContext context)
        {
            _context = context;
        }

        // GET: api/EndOfDayController2
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndOfDay>>> GetEndOfDay()
        {
            return await _context.EndOfDay.ToListAsync();
        }

        // GET: api/EndOfDayController2/5
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

        // PUT: api/EndOfDayController2/5
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

        // POST: api/EndOfDayController2
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<EndOfDay>> PostEndOfDay(EndOfDay endOfDay)
        {
            _context.EndOfDay.Add(endOfDay);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEndOfDay", new { id = endOfDay.Id }, endOfDay);
        }

        // DELETE: api/EndOfDayController2/5
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

        private bool EndOfDayExists(Guid id)
        {
            return _context.EndOfDay.Any(e => e.Id == id);
        }
    }
}
