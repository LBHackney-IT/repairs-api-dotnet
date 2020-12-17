using System.Data.Common;
using RepairsApi;
using RepairsApi.V1.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using RepairsApi.V1.Gateways;
using Microsoft.Extensions.Hosting;
using System;

namespace RepairsApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbBuilder = new DbContextOptionsBuilder();
                dbBuilder.UseSqlite("Data Source=:memory:");
                var context = new RepairsContext(dbBuilder.Options);
                services.AddSingleton(context);

                var serviceProvider = services.BuildServiceProvider();
                var dbContext = serviceProvider.GetRequiredService<RepairsContext>();

                dbContext.Database.EnsureCreated();

                //services.Remove(services.Where(s => s.ImplementationType == typeof(ApiGateway)).First());

                services.AddSingleton<IApiGateway, MockApiGateway>();
            })
            .ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
            .UseEnvironment("IntegrationTests");

        }
    }
}
