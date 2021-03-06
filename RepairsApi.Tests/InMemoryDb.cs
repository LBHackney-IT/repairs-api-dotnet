using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RepairsApi.V2.Infrastructure;
using System;

namespace RepairsApi.Tests
{
    public static class InMemoryDb
    {
        private static RepairsContext _context;

        public static RepairsContext Instance
        {
            get
            {
                if (_context == null)
                {
                    DbContextOptionsBuilder<RepairsContext> builder = new DbContextOptionsBuilder<RepairsContext>();
                    builder.EnableSensitiveDataLogging();
                    builder.ConfigureWarnings(options =>
                    {
                        options.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                    });
                    builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

                    _context = new RepairsContext(builder.Options);
                    _context.Database.EnsureCreated();
                }

                return _context;
            }
        }

        public static ITransactionManager TransactionManager => new TransactionManager(Instance);

        public static void Teardown()
        {
            _context = null;
        }
    }
}
