using System.Net.Http;
using RepairsApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NUnit.Framework;

namespace RepairsApi.Tests
{
    public class IntegrationTests<TStartup> where TStartup : class
    {
        protected HttpClient Client { get; private set; }
        protected RepairsContext RepairsContext { get; private set; }

        private MockWebApplicationFactory<TStartup> _factory;
        private IDbContextTransaction _transaction;
        private DbContextOptionsBuilder _builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _builder = new DbContextOptionsBuilder();
            _builder.UseSqlite("Data Source=:memory:");

        }

        [SetUp]
        public void BaseSetup()
        {
            _factory = new MockWebApplicationFactory<TStartup>();
            Client = _factory.CreateClient();
            RepairsContext = new RepairsContext(_builder.Options);
            RepairsContext.Database.EnsureCreated();
            _transaction = RepairsContext.Database.BeginTransaction();
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
