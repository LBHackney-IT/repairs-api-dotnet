using RepairsApi.V2.Domain;
using System.Security.Claims;

namespace RepairsApi.V2.Services
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal GetUser();
        bool IsUserPresent();
        bool HasGroup(string groupName);
        bool TryGetContractor(out string contractor);
        User GetHubUser();
    }
}
