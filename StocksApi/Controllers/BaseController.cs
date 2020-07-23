using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StocksApi.Model;

namespace StocksApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController<TContext, TSaveDto, TEntity> : ControllerBase
        where TContext : DbContext
        where TSaveDto : BaseSaveDto
        where TEntity : Entity
    {
        protected readonly IMapper _mapper;
        protected readonly TContext _dbContext;

        public BaseController(TContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        protected async Task<IActionResult> PutById(Guid id, DbSet<TEntity> dbSet, TSaveDto saveDto)
        {
            if (id == Guid.Empty || id != saveDto.Id)
                return BadRequest();

            var entity = await dbSet.SingleOrDefaultAsync(e => e.Id == id);

            if (entity == null)
                return NotFound();

            await dbSet.Persist(_mapper).InsertOrUpdateAsync(saveDto);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult<TEntity>> PostById(DbSet<TEntity> dbSet, TSaveDto saveDto, string getActionName)
        {
            var entity = await dbSet
                .Persist(_mapper)
                .InsertOrUpdateAsync(saveDto);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(getActionName, new { id = entity.Id }, entity);
        }

        protected async Task<IActionResult> DeleteById(DbSet<TEntity> dbSet, Guid id)
        {
            var entity = await dbSet.SingleOrDefaultAsync(e => e.Id == id);

            if (entity == null)
                return NotFound();

            dbSet.Remove(entity);
            
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult<TEntity>> GetById(DbSet<TEntity> dbSet, Guid id)
        {
            var entity = await dbSet.SingleOrDefaultAsync(e => e.Id == id);

            if (entity == null)
                return NotFound();

            return entity;
        }
    }
}
