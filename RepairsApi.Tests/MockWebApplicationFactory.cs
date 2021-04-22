using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using System;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Npgsql;
using V2_Generated_DRS;

namespace RepairsApi.Tests
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "tests")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "tests")]
    public class MockWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        private readonly string _connection = null;
        private string _userGroup = UserGroups.Agent;

        public MockWebApplicationFactory(string connection)
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
                services.RemoveAll<SOAP>();
                services.AddTransient<SOAP>(sp =>
                {
                    var mock = new Mock<SOAP>();
                    mock.Setup(x => x.openSessionAsync(It.IsAny<openSession>()))
                        .ReturnsAsync(new openSessionResponse { @return = new xmbOpenSessionResponse { status = responseStatus.success } });
                    mock.Setup(x => x.createOrderAsync(It.IsAny<createOrder>()))
                        .ReturnsAsync(new createOrderResponse { @return = new xmbCreateOrderResponse { status = responseStatus.success } });
                    return mock.Object;
                });
            })
            .UseEnvironment("IntegrationTests");
        }

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            if (_userGroup == UserGroups.Agent) client.SetAgent();
            if (_userGroup == UserGroups.Contractor) client.SetGroup(GetGroup(TestDataSeeder.Contractor));
            if (_userGroup == UserGroups.ContractManager) client.SetGroup(_userGroup);
        }

        protected void SetUserRole(string userGroup)
        {
            _userGroup = userGroup;
        }

        private static void InitialiseDB(ServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RepairsContext>();

            dbContext.Database.EnsureCreated();

            dbContext.SeedData();

            dbContext.SaveChanges();
        }

        protected ScopedContext GetContext()
        {
            return new ScopedContext(Services);
        }

        protected string GetGroup(string contractor)
        {
            using var ctx = GetContext();
            return ctx.DB.SecurityGroups.Where(sg => sg.ContractorReference == contractor).Select(sg => sg.GroupName).Single();
        }

        public async Task<(HttpStatusCode statusCode, TResponse response)> Get<TResponse>(string uri)
        {
            HttpResponseMessage result = await InternalGet(uri);

            TResponse response = await ProcessResponse<TResponse>(result);

            return (result.StatusCode, response);
        }

        public async Task<HttpStatusCode> Get(string uri)
        {
            HttpResponseMessage result = await InternalGet(uri);

            return result.StatusCode;
        }

        private async Task<HttpResponseMessage> InternalGet(string uri)
        {
            var client = CreateClient();

            var result = await client.GetAsync(new Uri(uri, UriKind.Relative));
            return result;
        }

        private static async Task<TResponse> ProcessResponse<TResponse>(HttpResponseMessage result)
        {
            var responseContent = await result.Content.ReadAsStringAsync();

            object parseResponse = JsonConvert.DeserializeObject(responseContent, typeof(TResponse));

            TResponse castedResponse = parseResponse != null && typeof(TResponse).IsAssignableFrom(parseResponse.GetType()) ? (TResponse) parseResponse : default;
            return castedResponse;
        }

        public async Task<(HttpStatusCode statusCode, TResponse response)> Post<TResponse>(string uri, object data)
        {
            HttpResponseMessage result = await InternalPost(uri, data);

            TResponse response = await ProcessResponse<TResponse>(result);
            return (result.StatusCode, response);
        }

        private async Task<HttpResponseMessage> InternalPost(string uri, object data, string role = "agent")
        {
            var client = CreateClient();
            if (!role.Equals("agent")) client.SetGroup(role);
            var serializedContent = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(new Uri(uri, UriKind.Relative), content);
            return result;
        }

        public async Task<HttpStatusCode> Post(string uri, object data, string role = "agent")
        {
            HttpResponseMessage result = await InternalPost(uri, data, role);
            return result.StatusCode;
        }
    }

    public sealed class ScopedContext : IDisposable
    {
        private readonly IServiceScope _scope;

        public RepairsContext DB { get; private set; }

        public ScopedContext(IServiceProvider services)
        {
            _scope = services.CreateScope();
            DB = _scope.ServiceProvider.GetRequiredService<RepairsContext>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
