using Microsoft.AspNetCore.Http;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RepairsApi.V2.MiddleWare
{
    public class InitialiseUserMiddleware
    {
        private const string HEADER = "X-Hackney-User";
        private readonly RequestDelegate _next;

        public InitialiseUserMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext httpContext, CurrentUserService userService)
        {
            userService.LoadUser(httpContext.Request.Headers[HEADER]);
            if (userService.IsUserPresent())
            {
                httpContext.User = MapUser(userService.GetUser());
            }
            await _next(httpContext);
        }

        private static ClaimsPrincipal MapUser(User user)
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));

            foreach (var group in user.Groups)
            {
                if (Groups.SecurityGroups.TryGetValue(group, out PermissionsModel perms))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, perms.SecurityGroup.ToString()));

                    if (!string.IsNullOrWhiteSpace(perms.ContractorReference))
                    {
                        identity.AddClaim(new Claim(CustomClaimTypes.CONTRACTOR, perms.ContractorReference));
                    }
                }
            }

            return new ClaimsPrincipal(identity);
        }
    }
}
