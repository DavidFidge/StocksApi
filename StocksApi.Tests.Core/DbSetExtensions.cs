using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using NSubstitute;

using StocksApi.Model;
using StocksApi.Tests.Core;

namespace StocksApi.Service.Tests
{
    public static class DbSetExtensions
    {
        public static DbSet<T> Initialize<T>(this DbSet<T> dbSet, IList<T> data) where T : Entity
        {
            var dataQueryable = data.AsQueryable();

            ((IQueryable<T>)dbSet).Expression
                .Returns(dataQueryable.Expression);

            ((IQueryable<T>)dbSet).ElementType
                .Returns(dataQueryable.ElementType);

            ((IQueryable<T>)dbSet).GetEnumerator()
                .Returns(data.GetEnumerator());

            ((IQueryable<T>)dbSet).Provider
                .Returns(new TestAsyncQueryProvider<T>(dataQueryable.Provider));

            return dbSet;
        }

        public static DbSet<T> WithAddRemove<T>(this DbSet<T> dbSet, IList<T> data) where T : Entity
        {
            dbSet
                .WithAdd(data)
                .WithRemove(data);

            return dbSet;
        }

        public static DbSet<T> WithAdd<T>(this DbSet<T> dbSet, IList<T> data) where T : Entity
        {
            dbSet
                .When(c => c.Add(Arg.Any<T>()))
                .Do(ci =>
                {
                    var entity = ci.Arg<T>();
                    entity.Id = Guid.NewGuid();
                    data.Add(entity);
                });

            dbSet
                .When(c => c.AddRange(Arg.Any<IEnumerable<T>>()))
                .Do(
                    ci =>
                    {
                        var list = ci.Arg<IEnumerable<T>>();
                        foreach (var item in list)
                        {
                            var entity = ci.Arg<T>();
                            entity.Id = Guid.NewGuid();
                            data.Add(item);
                        }
                    }
                );

            return dbSet;
        }

        public static DbSet<T> WithRemove<T>(this DbSet<T> dbSet, IList<T> data) where T : Entity
        {
            dbSet
                .When(c => c.Remove(Arg.Any<T>()))
                .Do(ci => data.Remove(ci.Arg<T>()));

            dbSet
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