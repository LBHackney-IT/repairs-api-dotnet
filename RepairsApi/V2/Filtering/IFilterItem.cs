using System;
using System.Linq.Expressions;

namespace RepairsApi.V2.Filtering
{
    public interface IFilterItem<TSearch, TQuery>
    {
        Expression<Func<TQuery, bool>> CreateExpression(TSearch search);

        bool IsValid(TSearch search);
    }
}
