using Microsoft.AspNetCore.Http;
using RepairsApi.V2.Services;
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

        public async Task Invoke(HttpContext httpContext, ICurrentUserLoader userInitialiser)
        {
            userInitialiser.LoadUser(httpContext.Request.Headers[HEADER]);
            await _next(httpContext);
        }
    }
}
