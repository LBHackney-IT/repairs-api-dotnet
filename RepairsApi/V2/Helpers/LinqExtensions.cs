using System.Collections.Generic;
using System.Linq;

namespace RepairsApi.V2.Helpers
{
    public static class LinqExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
    }
}
