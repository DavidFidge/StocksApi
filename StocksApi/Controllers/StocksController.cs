using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksApi.Data;
using StocksApi.Model;
using StocksApi.Service.Companies;

namespace StocksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly StocksContext _context;
        private readonly ICompanyInformation _companyInformation;

        public StocksController(StocksContext context, ICompanyInformation companyInformation)
        {
            _context = context;
            _companyInformation = companyInformation;
        }

        // GET: api/StocksController
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStock()
        {
            return await _context.Stock.ToListAsync();
        }

        // GET: api/StocksController/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStock(Guid id)
        {
            var stock = await _context.Stock.FindAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return stock;
        }

        // PUT: api/StocksController/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(Guid id, Stock stock)
        {
            if (id != stock.Id)
            {
                return BadRequest();
            }

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Stock>> Update()
        {
            await _companyInformation.Update();

            return NoContent();
        }

        // POST: api/StocksController
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(Stock stock)
        {
            _context.Stock.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStock", new { id = stock.Id }, stock);
        }

        // DELETE: api/StocksController/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Stock>> DeleteStock(Guid id)
        {
            var stock = await _context.Stock.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stock.Remove(stock);
            await _context.SaveChangesAsync();

            return stock;
        }

        private bool StockExists(Guid id)
        {
            return _context.Stock.Any(e => e.Id == id);
        }
    }
}
