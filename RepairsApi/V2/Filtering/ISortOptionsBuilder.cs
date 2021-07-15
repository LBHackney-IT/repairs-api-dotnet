using System;
using System.Linq.Expressions;

namespace RepairsApi.V2.Filtering
{
    public interface ISortOptionsBuilder<TQuery>
    {
        void AddSortOption<TProp>(string v, Expression<Func<TQuery, TProp>> p);
    }
}
