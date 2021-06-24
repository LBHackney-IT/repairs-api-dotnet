using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepairsApi.V2.Filtering
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSearch">A Model containing parameter values used for filtering</typeparam>
    /// <typeparam name="TQuery">The Object type used in an enumerable or queryable for filtering</typeparam>
    public class FilterBuilder<TSearch, TQuery> : IFilterBuilder<TSearch, TQuery>
    {
        private readonly List<IFilterItem<TSearch, TQuery>> _filterConfig = new List<IFilterItem<TSearch, TQuery>>();
        private readonly List<ISortConfig<TSearch, TQuery>> _sortConfig = new List<ISortConfig<TSearch, TQuery>>();

        /// <summary>
        /// Adds A Filter that allots filtering of TQuery based on values based from a TSearchModel
        /// </summary>
        /// <typeparam name="T">The Data type of the property in TSearch to be used fo filtering</typeparam>
        /// <param name="searchValueFunction">A Function returning the property from TSearch to be used for filtering</param>
        /// <param name="searchValidator">A Predicate determining if the value from TSearch should be used</param>
        /// <param name="filterFunction">A Function that given the TSearch property returns an expression to be used for filtering</param>
        /// <returns>The same filterbuilder to allow chaining</returns>
        public FilterBuilder<TSearch, TQuery> AddFilter<T>(Func<TSearch, T> searchValueFunction, Func<T, bool> searchValidator, Func<T, Expression<Func<TQuery, bool>>> filterFunction)
        {
            _filterConfig.Add(new FilterItem<T, TSearch, TQuery>(searchValueFunction, searchValidator, filterFunction));
            return this;
        }

        public IFilter<TQuery> BuildFilter(TSearch searchParameter)
        {
            return new Filter<TQuery>(
                _filterConfig.Where(c => c.IsValid(searchParameter)).Select(c => c.CreateExpression(searchParameter)),
                _sortConfig.FirstOrDefault(sc => sc.IsValid(searchParameter)));
        }

        internal FilterBuilder<TSearch, TQuery> AddSort(Func<TSearch, string> sort, Action<SortOptionsBuilder> sortBuilder)
        {
            SortOptionsBuilder builder = new SortOptionsBuilder(this, sort);

            sortBuilder.Invoke(builder);

            return this;
        }

        internal class SortOptionsBuilder
        {
            private readonly FilterBuilder<TSearch, TQuery> _filterBuilder;
            private readonly Func<TSearch, string> _sort;

            public SortOptionsBuilder(FilterBuilder<TSearch, TQuery> filterBuilder, Func<TSearch, string> sort)
            {
                _filterBuilder = filterBuilder;
                _sort = sort;
            }

            internal void AddSortOption<TProp>(string v, Expression<Func<TQuery, TProp>> p)
            {
                _filterBuilder._sortConfig.Add(new SortItem<TProp, TQuery, TSearch>(v, p, _sort));
            }
        }
    }

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

        public bool IsValid(TSearch searchParameter)
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

    internal interface ISortConfig<TSearch, TQuery> : ISortItem<TQuery>
    {
        bool IsValid(TSearch searchParameter);
    }

    public interface ISortItem<TQuery>
    {
        IQueryable<TQuery> AddOrdering(IQueryable<TQuery> filtered);
    }
}
