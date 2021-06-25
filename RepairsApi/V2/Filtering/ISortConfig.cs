namespace RepairsApi.V2.Filtering
{
    internal interface ISortConfig<TSearch, TQuery> : ISortItem<TQuery>
    {
        bool IsValid(TSearch searchParameter);
    }
}
