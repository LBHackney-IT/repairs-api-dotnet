using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace RepairsApi.V2.Generated.DRS.BackgroundService
{
#pragma warning disable CA2000
    public class DrsBackgroundServiceLogger
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public DrsBackgroundServiceLogger(RequestDelegate next)
        {
            this._next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
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
                    var newBody = originalContent.Replace("ns1:bookingConfirmation", "ns1bookingConfirmation");
                    logger.LogInformation("Transformed request: {request}", newBody);
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
    }
}
