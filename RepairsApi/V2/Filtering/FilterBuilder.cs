using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepairsApi.V2.Filtering
{
    public class FilterBuilder<TSearch, TQuery> : IFilterBuilder<TSearch, TQuery>
    {
        private List<IFilterItem<TSearch, TQuery>> _config = new List<IFilterItem<TSearch, TQuery>>();

        public FilterBuilder<TSearch, TQuery> AddFilter<T>(Func<TSearch, T> searchValueFunction, Func<T, bool> searchValidator, Func<T, Expression<Func<TQuery, bool>>> filterFunction)
        {
           _config.Add(new FilterItem<T, TSearch, TQuery>(searchValueFunction, searchValidator, filterFunction));
           return this;
        }

        public IFilter<TQuery> BuildFilter(TSearch searchParameter)
        {
            return new Filter<TQuery>(_config.Where(c => c.IsValid(searchParameter)).Select(c => c.CreateExpression(searchParameter)));
        }
    }
}
