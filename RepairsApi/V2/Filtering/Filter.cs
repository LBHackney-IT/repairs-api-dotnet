using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepairsApi.V2.Filtering
{
    public class Filter<TQuery> : IFilter<TQuery>
    {
        private readonly IEnumerable<Expression<Func<TQuery, bool>>> _predicates;

        public Filter(IEnumerable<Expression<Func<TQuery, bool>>> predicates)
        {
            _predicates = predicates;
        }

        public IQueryable<TQuery> Apply(IQueryable<TQuery> query)
        {
            return _predicates.Aggregate(query, (q, pred) => q.Where(pred));
        }

        public IEnumerable<TQuery> Apply(IEnumerable<TQuery> query)
        {
            return _predicates.Aggregate(query, (q, pred) => q.Where(pred.Compile()));
        }
    }
}
