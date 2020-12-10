using RepairsApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;

namespace RepairsApi.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        private IDbContextTransaction _transaction;
        protected RepairsContext RepairsContext { get; private set; }

        [SetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(ConnectionString.TestDatabase());
            RepairsContext = new RepairsContext(builder.Options);

            RepairsContext.Database.EnsureCreated();
            _transaction = RepairsContext.Database.BeginTransaction();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
