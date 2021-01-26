using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using StocksApi.Migrations.Sqlite;

namespace StocksApi.ConsoleApp.Sqlite
{
    public class SqliteStocksContextFactory : IDesignTimeDbContextFactory<SqliteStocksContext>
    {
        public SqliteStocksContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteStocksContext>();

            optionsBuilder.UseSqlite("Filename=Stocks.db");

            return new SqliteStocksContext(optionsBuilder.Options);
        }
    }
}
