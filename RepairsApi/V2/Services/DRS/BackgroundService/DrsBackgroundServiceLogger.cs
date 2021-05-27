using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RepairsApi.V2.Generated.DRS.BackgroundService
{
    public class DrsBackgroundServiceLogger
    {
        private readonly RequestDelegate _next;

        public DrsBackgroundServiceLogger(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<DrsBackgroundServiceLogger> logger)
        {
            if (httpContext.Request.Path.Equals((PathString) "/Service.asmx"))
            {
                using var reader = new StreamReader(httpContext.Request.Body);
                var bodyText = await reader.ReadToEndAsync();
                logger.LogInformation("DRS soap request received: {M}", bodyText);

            }
            await _next(httpContext);
        }
    }
}
