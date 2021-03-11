using System.Security.Claims;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.Services
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal GetUser();
        bool IsUserPresent();
        bool HasGroup(string groupName);
        bool TryGetContractor(out string contractor);
        HubUserModel GetHubUser();
    }
}
