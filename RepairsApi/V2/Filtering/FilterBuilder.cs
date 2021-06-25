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
                _sortConfig.FirstOrDefault(sc => sc.SetOrderExpression(searchParameter)));
        }

        public FilterBuilder<TSearch, TQuery> AddSort(Func<TSearch, string> sort, Action<ISortOptionsBuilder<TQuery>> sortBuilder)
        {
            SortOptionsBuilder builder = new SortOptionsBuilder(this, sort);

            sortBuilder.Invoke(builder);

            return this;
        }

        internal class SortOptionsBuilder : ISortOptionsBuilder<TQuery>
        {
            private readonly FilterBuilder<TSearch, TQuery> _filterBuilder;
            private readonly Func<TSearch, string> _sort;

            public SortOptionsBuilder(FilterBuilder<TSearch, TQuery> filterBuilder, Func<TSearch, string> sort)
            {
                _filterBuilder = filterBuilder;
                _sort = sort;
            }

            public void AddSortOption<TProp>(string v, Expression<Func<TQuery, TProp>> p)
            {
                _filterBuilder._sortConfig.Add(new SortItem<TProp, TQuery, TSearch>(v, p, _sort));
            }
        }
    }
}
