using Microsoft.EntityFrameworkCore;

using StocksApi.Models;

namespace StocksApi.Data
{
    public class StocksDatabaseContext : DbContext
    {
        public StocksDatabaseContext(DbContextOptions<StocksDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<EndOfDay> EndOfDay { get; set; }
        public DbSet<Stock> Stock { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Stocks.db");
        }
    }
}
