using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepairsApi.V2.Filtering
{
    public class Filter<TQuery> : IFilter<TQuery>
    {
        private readonly IEnumerable<Expression<Func<TQuery, bool>>> _predicates;
        private readonly ISortItem<TQuery> _sortConfig;

        public Filter(IEnumerable<Expression<Func<TQuery, bool>>> predicates, ISortItem<TQuery> sortConfig)
        {
            _predicates = predicates;
            _sortConfig = sortConfig;
        }

        public IQueryable<TQuery> Apply(IQueryable<TQuery> query)
        {
            var filtered = _predicates.Aggregate(query, (q, pred) => q.Where(pred));

            if (_sortConfig is null)
            {
                return filtered;
            }

            return _sortConfig.AddOrdering(filtered);
        }
    }
}
