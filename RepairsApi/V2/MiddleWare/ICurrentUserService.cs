using RepairsApi.V2.Domain;

namespace RepairsApi.V2.MiddleWare
{
    public interface ICurrentUserService
    {
        User GetUser();
        bool IsUserPresent();
    }
}
