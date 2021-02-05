namespace RepairsApi.V2.MiddleWare
{
    public interface ICurrentUserLoader
    {
        void LoadUser(string jwt);
    }
}