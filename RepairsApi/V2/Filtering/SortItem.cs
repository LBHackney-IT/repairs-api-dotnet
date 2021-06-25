using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepairsApi.V2.Filtering
{
    internal class SortItem<TProp, TQuery, TSearch> : ISortConfig<TSearch, TQuery>
    {
        private readonly string _queryAlias;
        private readonly Expression<Func<TQuery, TProp>> _sortProperty;
        private readonly Func<TSearch, string> _sortQueryFinder;
        private const string ASC = "asc";
        private const string DESC = "desc";
        private Func<IQueryable<TQuery>, IQueryable<TQuery>> _orderExpression;

        public SortItem(string queryAlias, Expression<Func<TQuery, TProp>> sortProperty, Func<TSearch, string> sortQuery)
        {
            _queryAlias = queryAlias;
            _sortProperty = sortProperty;
            _sortQueryFinder = sortQuery;
        }

        public IQueryable<TQuery> AddOrdering(IQueryable<TQuery> filtered)
        {
            return _orderExpression?.Invoke(filtered);
        }

        public bool SetOrderExpression(TSearch searchParameter)
        {
            try
            {
                var sortQuery = _sortQueryFinder(searchParameter);
                var value = sortQuery.Split(':');
                var alias = value[0];
                var direction = value[1];

                if (alias.ToLower() != _queryAlias.ToLower()) return false;
                if (direction != ASC && direction != DESC) return false;

                SetOrderExpression(direction);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetOrderExpression(string direction)
        {
            if (direction == ASC)
            {
                _orderExpression = q => q.OrderBy(_sortProperty);
            }
            else
            {
                _orderExpression = q => q.OrderByDescending(_sortProperty);
            }
        }
    }
}
