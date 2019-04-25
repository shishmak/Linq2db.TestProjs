using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace TestLinq2Db
{
    public static class LinqExtensions
    {   //
        //public static int Update<T>([NotNull] this IUpdatable<T> source)
        //{
        //    if (source == null) throw new ArgumentNullException(nameof(source));
        //
        //    var query = ((Updatable<T>)source).Query;
        //
        //    var currentQuery = ProcessSourceQueryable?.Invoke(query) ?? query;
        //
        //    return currentQuery.Provider.Execute<int>(
        //        Expression.Call(
        //            null,
        //            _updateMethodInfo4.MakeGenericMethod(typeof(T)),
        //            currentQuery.Expression));
        //}
    }
}
