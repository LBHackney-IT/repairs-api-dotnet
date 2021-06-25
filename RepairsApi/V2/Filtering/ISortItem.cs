using System.Linq;

namespace RepairsApi.V2.Filtering
{
    public interface ISortItem<TQuery>
    {
        IQueryable<TQuery> AddOrdering(IQueryable<TQuery> filtered);
    }
}
