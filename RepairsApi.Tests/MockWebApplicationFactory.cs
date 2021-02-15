using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using System;
using System.Data.Common;
using System.Net.Http;

namespace RepairsApi.Tests
{
    public class MockWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        private readonly DbConnection _connection = null;

        public MockWebApplicationFactory(DbConnection connection)
        {
            _connection = connection;
        }

        public MockWebApplicationFactory()
            : base()
        {

        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<RepairsContext>));

                services.AddDbContext<RepairsContext>(options =>
                {
                    if (_connection != null)
                    {
                        options.UseNpgsql(_connection)
                        .UseSnakeCaseNamingConvention()
                            .UseLazyLoadingProxies();
                    }
                    else
                    {
                        options.UseInMemoryDatabase("integration")
                            .UseLazyLoadingProxies()
                            .UseSnakeCaseNamingConvention();
                        options.ConfigureWarnings(warningOptions =>
                        {
                            warningOptions.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                        });
                    }
                });

                var serviceProvider = services.BuildServiceProvider();
                InitialiseDB(serviceProvider);

                services.RemoveAll<IApiGateway>();
                services.AddTransient<IApiGateway, MockApiGateway>();
            })
            .UseEnvironment("IntegrationTests");
        }

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            client.DefaultRequestHeaders.Add("X-Hackney-User", TestUserInformation.JWT);
        }

        private static void InitialiseDB(ServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RepairsContext>();

                dbContext.Database.EnsureCreated();

                dbContext.SeedData();

                dbContext.SaveChanges();
            }
        }

        protected void WithContext(Action<RepairsContext> action)
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RepairsContext>();

                action(dbContext);
            }
        }
    }
}
