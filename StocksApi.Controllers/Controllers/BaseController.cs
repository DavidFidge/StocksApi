using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StocksApi.Model;

namespace StocksApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController<TContext, TDto, TSaveDto, TEntity> : ControllerBase
        where TContext : DbContext
        where TDto : BaseDto
        where TSaveDto : BaseDto
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

            _mapper.Map(saveDto, entity);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<IActionResult> PostById(DbSet<TEntity> dbSet, TSaveDto saveDto, string getActionName)
        {
            var entity = await PostById(dbSet, saveDto);

            return GetCreatedAtAction(getActionName, entity);
        }

        protected IActionResult GetCreatedAtAction(string getActionName, TEntity entity)
        {
            return CreatedAtAction(getActionName, new { id = entity.Id }, null);
        }

        protected async Task<TEntity> PostById(DbSet<TEntity> dbSet, TSaveDto saveDto)
        {
            var entity = _mapper.Map<TSaveDto, TEntity>(saveDto);

            dbSet.Add(entity);

            await _dbContext.SaveChangesAsync();
            return entity;
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

        protected async Task<ActionResult<TDto>> GetById(DbSet<TEntity> dbSet, Guid id)
        {
            var entity = await dbSet.SingleOrDefaultAsync(e => e.Id == id);

            if (entity == null)
                return NotFound();

            return _mapper.Map<TDto>(entity);
        }
    }
}
