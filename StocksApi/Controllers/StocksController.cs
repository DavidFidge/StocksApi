using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

using StocksApi.Data;
using StocksApi.Model;
using StocksApi.Service.Companies;

namespace StocksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : BaseController<StocksContext, SaveStockDto, Stock>
    {
        private readonly ICompanyInformation _companyInformation;

        public StocksController(StocksContext dbContext, ICompanyInformation companyInformation, IMapper mapper)
            : base(dbContext, mapper)
        {
            _companyInformation = companyInformation;
        }

        [HttpGet]
        [EnableQuery(PageSize = 50)]
        [ODataRoute]
        public IQueryable<Stock> GetStock()
        {
            return _dbContext.Stock;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStock(Guid id)
        {
            return await GetById(_dbContext.Stock, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(Guid id, SaveStockDto saveStockDto)
        {
            return await PutById(id, _dbContext.Stock, saveStockDto);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateFromThirdParty()
        {
            await _companyInformation.Update(_dbContext);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(SaveStockDto saveStockDto)
        {
            return await PostById(_dbContext.Stock, saveStockDto, nameof(GetStock));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(Guid id)
        {
            return await DeleteById(_dbContext.Stock, id);
        }
    }
}
