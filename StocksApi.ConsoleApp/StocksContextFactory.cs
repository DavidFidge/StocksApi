using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StocksApi.Data;

namespace StocksApi.ConsoleApp
{
    public class StocksContextFactory : IDesignTimeDbContextFactory<StocksContext>
    {
        public StocksContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StocksContext>();
                optionsBuilder.UseSqlite("Filename=Stocks.db");

            return new StocksContext(optionsBuilder.Options);
        }
    }
}
