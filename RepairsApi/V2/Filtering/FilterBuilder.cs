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
        private List<IFilterItem<TSearch, TQuery>> _config = new List<IFilterItem<TSearch, TQuery>>();

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
           _config.Add(new FilterItem<T, TSearch, TQuery>(searchValueFunction, searchValidator, filterFunction));
           return this;
        }

        public IFilter<TQuery> BuildFilter(TSearch searchParameter)
        {
            return new Filter<TQuery>(_config.Where(c => c.IsValid(searchParameter)).Select(c => c.CreateExpression(searchParameter)));
        }
    }
}
