using Microsoft.EntityFrameworkCore;

using StocksApi.Data;

namespace StocksApi.Migrations.Sqlite
{
    public class SqliteStocksContext : StocksContext
    {
        public SqliteStocksContext()
        {
        }

        public SqliteStocksContext(DbContextOptions options)
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
