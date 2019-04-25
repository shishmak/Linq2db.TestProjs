using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestLinq2Db
{
    public static class SqlExpressions
    {
        [Sql.Expression("{0} IN {1} ", PreferServerSide = true)]
        public static bool In<T>(this T item, IEnumerable<T> items)
        {
            return items.Contains(item); // this code will run if we execute the method locally
        }

        [Sql.Expression("{0} NOT IN {1} ", PreferServerSide = true)]
        public static bool NotIn<T>(this T item, IEnumerable<T> items)
        {
            return !items.Contains(item); // this code will run if we execute the method locally
        }

        public class LeftOuter<T1,T2> {
           public T1 Left { get; set; }
           public T2 Right { get; set; }
        }

        public static IQueryable<LeftOuter<TOuter, TInner>> LeftJoin<TOuter, TInner, TKey>(this IQueryable<TOuter> outer, IQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector)
           => outer.GroupJoin(inner, outerKeySelector, innerKeySelector, (o, i) => new { o, i })
                .SelectMany(sm => sm.i.DefaultIfEmpty(), (sm, i) => new LeftOuter<TOuter, TInner> { Left = sm.o, Right = i });

    }
}
