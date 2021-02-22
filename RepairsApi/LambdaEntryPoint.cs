using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;

namespace RepairsApi
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration(config =>
                {
                    config.AddGroups();
                })
                .UseStartup<Startup>();
        }
    }
}
