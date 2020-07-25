using Microsoft.EntityFrameworkCore;

using StocksApi.Model;

namespace StocksApi.Data
{
    public class StocksContext : DbContext
    {
        public virtual DbSet<EndOfDay> EndOfDay { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<PortfolioManager> PortfolioManagers { get; set; }
        public virtual DbSet<Portfolio> Portfolios { get; set; }
        public virtual DbSet<Holding> Holdings { get; set; }

        public StocksContext()
        {
        }

        public StocksContext(DbContextOptions<StocksContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            optionsBuilder.UseSqlite("Filename=Stocks.db");
        }
    }
}
