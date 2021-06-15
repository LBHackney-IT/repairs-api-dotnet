using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Authorisation;

namespace RepairsApi.V2.Services.DRS.BackgroundService
{
#pragma warning disable CA2000
    public class DrsBackgroundServiceLogger
    {
        private readonly RequestDelegate _next;

        public DrsBackgroundServiceLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<DrsBackgroundServiceLogger> logger)
        {
            var request = context.Request;
            if (request.Path.Equals((PathString) "/Service.asmx"))
            {
                //get the request body and put it back for the downstream items to read
                var stream = request.Body;// currently holds the original stream
                var streamReader = new StreamReader(stream);
                var originalContent = await streamReader.ReadToEndAsync();
                logger.LogInformation("Received request: {request}", originalContent);
                var notModified = true;
                try
                {
                    var newBody = originalContent.Replace("ns1:bookingConfirmation", "bookingConfirmationContainer");
                    context.Request.Headers["SOAPAction"] = "bookingConfirmationContainer";
                    logger.LogInformation("Transformed request: {request}", newBody);

                    //add in service user permissions
                    context.User = CreateServiceUser();

                    var requestContent = new StringContent(newBody, Encoding.UTF8, "application/json");
                    await stream.DisposeAsync();
                    stream = await requestContent.ReadAsStreamAsync();//modified stream
                    notModified = false;
                }
                catch
                {
                    //No-op or log error
                }
                if (notModified)
                {
                    //put original data back for the downstream to read
                    var requestData = Encoding.UTF8.GetBytes(originalContent);
                    stream = new MemoryStream(requestData);
                }

                request.Body = stream;
            }
            await _next(context);
        }

        private static ClaimsPrincipal CreateServiceUser()
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.Email, "serviceUser"));
            identity.AddClaim(new Claim(ClaimTypes.Name, "serviceUser"));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, "serviceUser"));
            identity.AddClaim(new Claim(CustomClaimTypes.RaiseLimit, "0"));
            identity.AddClaim(new Claim(CustomClaimTypes.VaryLimit, "0"));
            identity.AddClaim(new Claim(ClaimTypes.Role, UserGroups.Service));

            return new ClaimsPrincipal(identity);
        }
    }
}
