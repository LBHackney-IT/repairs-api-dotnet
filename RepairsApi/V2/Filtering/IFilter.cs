using System.Collections.Generic;
using System.Linq;

namespace RepairsApi.V2.Filtering
{
    public interface IFilter<TQuery>
    {
        IQueryable<TQuery> Apply(IQueryable<TQuery> query);
    }
}
