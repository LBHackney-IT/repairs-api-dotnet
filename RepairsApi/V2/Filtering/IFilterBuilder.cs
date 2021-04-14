namespace RepairsApi.V2.Filtering
{
    public interface IFilterBuilder<TSearch, TQuery>
    {
        IFilter<TQuery> BuildFilter(TSearch searchParameter);
    }
}
