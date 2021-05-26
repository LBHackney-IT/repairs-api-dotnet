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
using RepairsApi.Tests.Helpers;
using NUnit.Framework;
using RepairsApi.V2.Services;
using Notify.Interfaces;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using System.Collections.Generic;

namespace RepairsApi.Tests
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "tests")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "tests")]
    public class MockWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        private readonly string _connection = null;
        private string _userGroup = UserGroups.Agent;
        private readonly SoapMock _soapMock = new SoapMock();
        protected SoapMock SoapMock => _soapMock;

        private readonly NotifyWrapper _notifyMock = new NotifyWrapper();
        protected NotifyWrapper NotifyMock => _notifyMock;

        private readonly LogAggregator _log = new LogAggregator();
        protected LogAggregator Logs => _log;

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
                services.RemoveAll<IAsyncNotificationClient>();
                services.AddTransient(sp => _notifyMock.Object);
                services.AddTransient(sp => _soapMock.Object);
                services.RemoveAll(typeof(ILogger<>));
                services.AddTransient(typeof(ILogger<>), typeof(MockLogger<>));
                services.AddSingleton(_log);
            })
            .UseEnvironment("IntegrationTests");
        }

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            switch (_userGroup)
            {
                case UserGroups.Agent: client.SetAgent(); break;
                case UserGroups.Contractor: client.SetGroups(GetGroup(TestDataSeeder.Contractor)); break;
                default: client.SetGroups(_userGroup); break;
            }
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

        protected string[] GetGroup(string contractor)
        {
            using var ctx = GetContext();
            return ctx.DB.SecurityGroups.Where(sg => sg.ContractorReference == contractor).Select(sg => sg.GroupName).First();
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

            try
            {
                object parseResponse = JsonConvert.DeserializeObject(responseContent, typeof(TResponse));
                TResponse castedResponse = parseResponse != null && typeof(TResponse).IsAssignableFrom(parseResponse.GetType()) ? (TResponse) parseResponse : default;
                return castedResponse;
            }
            catch (Exception e) when (e is JsonSerializationException || e is JsonReaderException)
            {
                throw new Exception($"Result Serialisation Failed. Response Had Code {result.StatusCode}", e);
            }
        }

        public async Task<(HttpStatusCode statusCode, TResponse response)> Post<TResponse>(string uri, object data)
        {
            HttpResponseMessage result = await InternalPost(uri, data);

            TResponse response = await ProcessResponse<TResponse>(result);
            return (result.StatusCode, response);
        }

        private async Task<HttpResponseMessage> InternalPost(string uri, object data)
        {
            var client = CreateClient();

            var serializedContent = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(new Uri(uri, UriKind.Relative), content);
            return result;
        }

        public async Task<HttpStatusCode> Post(string uri, object data)
        {
            HttpResponseMessage result = await InternalPost(uri, data);
            return result.StatusCode;
        }

        protected void VerifyEmailSent(string templateId, string recipient = null)
        {
            NotifyMock.LastEmail.Should().Match<EmailRecord>(r => r.TemplateId == templateId && (recipient == null || recipient == r.Email));
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
