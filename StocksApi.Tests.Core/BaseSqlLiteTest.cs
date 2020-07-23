using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace StocksApi.Service.Tests
{
    public abstract class BaseSqlLiteTest<T, TContext> : BaseTest<T>, IDisposable
    where TContext : DbContext
    {
        protected DbConnection Connection;
        protected DbContextOptions<TContext> ContextOptions { get; set; }

        public override void Setup()
        {
            base.Setup();

            Connection = CreateInMemoryDatabase();

            ContextOptions = new DbContextOptionsBuilder<TContext>()
                .UseSqlite(Connection)
                .Options;
        }

        private DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();
        
            return connection;
        }

        public void Dispose() => Connection.Dispose();
    }
}
