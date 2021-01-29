using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.Domain
{
    public static class EnumerableExtensions
    {
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
        {
            if (source is null) return;

            foreach (var item in source)
            {
                await action(item);
            }
        }
    }
}
