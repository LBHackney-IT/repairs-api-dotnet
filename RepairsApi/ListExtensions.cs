using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi
{
    public static class ListExtensions
    {
        public static List<TResult> MapList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> map)
        {
            if (source is null) return new List<TResult>();

            return source.Select(map).ToList();
        }
    }
}
