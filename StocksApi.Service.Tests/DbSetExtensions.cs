using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using NSubstitute;

namespace StocksApi.Service.Tests
{
    public static class DbSetExtensions
    {
        public static DbSet<T> Initialize<T>(this DbSet<T> dbSet, IList<T> data) where T : class
        {
            var dataQueryable = data.AsQueryable();

            ((IQueryable<T>)dbSet).Provider.Returns(dataQueryable.Provider);
            ((IQueryable<T>)dbSet).Expression.Returns(dataQueryable.Expression);
            ((IQueryable<T>)dbSet).ElementType.Returns(dataQueryable.ElementType);
            ((IQueryable<T>)dbSet).GetEnumerator().Returns(data.GetEnumerator());

            return dbSet;
        }

        public static DbSet<T> WithAdd<T>(this DbSet<T> dbSet, IList<T> data, DbContext context) where T : class
        {
            context
                .When(c => c.Add(Arg.Any<T>()))
                .Do(ci => data.Add(ci.Arg<T>()));

            return dbSet;
        }
    }
}