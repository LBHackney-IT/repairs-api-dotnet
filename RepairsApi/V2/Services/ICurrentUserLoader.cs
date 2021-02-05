namespace RepairsApi.V2.Services
{
    public interface ICurrentUserLoader
    {
        void LoadUser(string jwt);
    }
}
