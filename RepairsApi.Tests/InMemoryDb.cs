using Microsoft.EntityFrameworkCore;
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
                    var builder = new DbContextOptionsBuilder<RepairsContext>();
                    builder.EnableSensitiveDataLogging();
                    builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

                    _context = new RepairsContext(builder.Options);
                    _context.Database.EnsureCreated();
                }

                return _context;
            }
        }

        public static void Teardown()
        {
            _context = null;
        }
    }
}
