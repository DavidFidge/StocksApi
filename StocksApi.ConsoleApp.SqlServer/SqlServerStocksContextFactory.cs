using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using StocksApi.Migrations.SqlServer;

namespace StocksApi.ConsoleApp.SqlServer
{
    public class SqlServerStocksContextFactory : IDesignTimeDbContextFactory<SqlServerStocksContext>
    {
        public SqlServerStocksContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerStocksContext>();

            optionsBuilder.UseSqlServer();

            return new SqlServerStocksContext(optionsBuilder.Options);
        }
    }
}
