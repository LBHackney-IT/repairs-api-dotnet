using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{

    public class RepairsContext : DbContext
    {
        public RepairsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RateScheduleItem> RateScheduleItems { get; set; }
    }
}
