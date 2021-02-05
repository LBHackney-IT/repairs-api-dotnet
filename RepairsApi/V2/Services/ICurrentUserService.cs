using RepairsApi.V2.Domain;

namespace RepairsApi.V2.Services
{
    public interface ICurrentUserService
    {
        User GetUser();
        bool IsUserPresent();
    }
}
