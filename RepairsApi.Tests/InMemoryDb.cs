using Microsoft.EntityFrameworkCore;
using RepairsApi.V1.Infrastructure;
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
                    DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
                    builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

                    _context = new RepairsContext(builder.Options);
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
