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
            await userService.LoadUser(httpContext.Request.Headers[HEADER]);
            httpContext.User = userService.GetUser();
            await _next(httpContext);
        }
    }
}
