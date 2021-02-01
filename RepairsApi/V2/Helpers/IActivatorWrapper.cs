namespace RepairsApi.V2.Helpers
{
    public interface IActivatorWrapper<T>
    {
        T CreateInstance<TConcrete>() where TConcrete : T;
    }
}
