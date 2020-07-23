using Microsoft.EntityFrameworkCore;

namespace StocksApi.Service.Tests
{
    public abstract class BaseSqlLiteTest<T, TContext> : BaseTest<T>
    where TContext : DbContext
    {
        protected DbContextOptions<TContext> ContextOptions { get; set; }

        public override void Setup()
        {
            base.Setup();

            ContextOptions = new DbContextOptionsBuilder<TContext>()
                .UseSqlite("Filename=Test.db")
                .Options;
        }
    }
}
