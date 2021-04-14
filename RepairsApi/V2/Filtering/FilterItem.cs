using System;
using System.Linq.Expressions;

namespace RepairsApi.V2.Filtering
{
    public class FilterItem<T, TSearch, TQuery> : IFilterItem<TSearch, TQuery>
    {
        private Func<TSearch, T> _searchValueFunction;
        private readonly Func<T, bool> _searchValidator;
        private Func<T, Expression<Func<TQuery, bool>>> _filterFunction;

        public FilterItem(Func<TSearch, T> p, Func<T, bool> searchValidator, Func<T, Expression<Func<TQuery, bool>>> predicate)
        {
            this._searchValueFunction = p;
            _searchValidator = searchValidator;
            this._filterFunction = predicate;
        }

        public Expression<Func<TQuery, bool>> CreateExpression(TSearch search)
        {
            T value = _searchValueFunction(search);

            var filterExpression = _filterFunction(value);

            return filterExpression;
        }

        public bool IsValid(TSearch search)
        {
            T value = _searchValueFunction(search);
            return _searchValidator(value);
        }
    }
}
