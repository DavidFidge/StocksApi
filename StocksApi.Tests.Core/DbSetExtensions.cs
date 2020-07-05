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

        public static DbSet<T> WithAddRemove<T>(this DbSet<T> dbSet, IList<T> data, DbContext context) where T : class
        {
            dbSet
                .WithAdd(data, context)
                .WithRemove(data, context);

            return dbSet;
        }

        public static DbSet<T> WithAdd<T>(this DbSet<T> dbSet, IList<T> data, DbContext context) where T : class
        {
            context
                .When(c => c.Add(Arg.Any<T>()))
                .Do(ci => data.Add(ci.Arg<T>()));

            context
                .When(c => c.AddRange(Arg.Any<IEnumerable<T>>()))
                .Do(
                    ci =>
                    {
                        var list = ci.Arg<IEnumerable<T>>();
                        foreach (var item in list)
                        {
                            data.Add(item);
                        }
                    }
                );

            return dbSet;
        }

        public static DbSet<T> WithRemove<T>(this DbSet<T> dbSet, IList<T> data, DbContext context) where T : class
        {
            context
                .When(c => c.Remove(Arg.Any<T>()))
                .Do(ci => data.Remove(ci.Arg<T>()));

            context
                .When(c => c.RemoveRange(Arg.Any<IEnumerable<T>>()))
                .Do(
                    ci =>
                    {
                        var list = ci.Arg<IEnumerable<T>>();
                        foreach (var item in list)
                        {
                            data.Remove(item);
                        }
                    }
                );

            return dbSet;
        }
    }
}