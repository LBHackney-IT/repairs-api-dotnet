using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;
using RepairsApi.V2.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace RepairsApi.Tests
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
    public class IntegrationTests<TStartup> where TStartup : class
    {
        protected HttpClient Client { get; private set; }
        protected RepairsContext RepairsContext { get; private set; }

        private MockWebApplicationFactory _factory;
        private NpgsqlConnection _connection = null;
        private DbContextOptionsBuilder _builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _builder = new DbContextOptionsBuilder();

            try
            {
                _connection = new NpgsqlConnection(ConnectionString.TestDatabase());
                _connection.Open();
                var npgsqlCommand = _connection.CreateCommand();
                npgsqlCommand.CommandText = "SET deadlock_timeout TO 30";
                npgsqlCommand.ExecuteNonQuery();

                _builder.UseNpgsql(_connection);
            }
            catch
            {
                _connection.Dispose();
                _connection = null;
                _builder.UseSqlite("Data Source=:memory:");
            }

        }

        [SetUp]
        public void BaseSetup()
        {
            _factory = new MockWebApplicationFactory(_connection);
            Client = _factory.CreateClient();
            RepairsContext = new RepairsContext(_builder.Options);
            RepairsContext.Database.EnsureCreated();
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
        }
    }
}
