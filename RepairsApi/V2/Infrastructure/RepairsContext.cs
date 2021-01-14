using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V2.Infrastructure
{

    public class RepairsContext : DbContext
    {
        public RepairsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkOrderComplete> WorkOrderCompletes { get; set; }
        public DbSet<JobStatusUpdate> JobStatusUpdates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkOrder>()
                .Property(wo => wo.Id)
                .HasIdentityOptions(startValue: 10000000);

            modelBuilder.Seed();
        }
    }
}
