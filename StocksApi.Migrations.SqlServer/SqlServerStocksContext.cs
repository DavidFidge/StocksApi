using System;

using Microsoft.EntityFrameworkCore;

using StocksApi.Data;

namespace StocksApi.Migrations.SqlServer
{
    public class SqlServerStocksContext : StocksContext
    {

        public SqlServerStocksContext()
        {
        }

        public SqlServerStocksContext(DbContextOptions options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            optionsBuilder.UseSqlServer();
        }
    }
}
