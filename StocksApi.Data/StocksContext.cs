using Microsoft.EntityFrameworkCore;
using StocksApi.Model;

namespace StocksApi.Data
{
    public class StocksContext : DbContext
    {
        public DbSet<EndOfDay> EndOfDay { get; set; }
        public DbSet<Stock> Stock { get; set; }

        public StocksContext(DbContextOptions<StocksContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Stocks.db");
        }
    }
}
