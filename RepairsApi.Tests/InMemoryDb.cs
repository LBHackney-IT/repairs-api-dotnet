using Microsoft.EntityFrameworkCore;
using RepairsApi.V1.Infrastructure;

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
                    builder.UseInMemoryDatabase("default");

                    _context = new RepairsContext(builder.Options);
                }

                return _context;
            }
        }
    }
}
