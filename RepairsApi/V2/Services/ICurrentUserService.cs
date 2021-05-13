using System.Collections.Generic;
using System.Security.Claims;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.Services
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal GetUser();
        bool IsUserPresent();
        bool HasGroup(string groupName);
        List<string> GetContractors();
        bool HasAnyGroup(params string[] groups);
    }
}
