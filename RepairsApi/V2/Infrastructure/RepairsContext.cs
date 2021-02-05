using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V2.Infrastructure
{

    public class RepairsContext : DbContext
    {
        private readonly DataImporter _dataImporter;

        public RepairsContext(
            DbContextOptions options,
            DataImporter dataImporter
        ) : base(options)
        {
            _dataImporter = dataImporter;
        }

        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkElement> WorkElements { get; set; }
        public DbSet<WorkOrderComplete> WorkOrderCompletes { get; set; }
        public DbSet<JobStatusUpdate> JobStatusUpdates { get; set; }
        public DbSet<ScheduleOfRates> SORCodes { get; set; }
        public DbSet<SORPriority> SORPriorities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkOrder>()
                .Property(wo => wo.Id)
                .HasIdentityOptions(startValue: 10000000);

            modelBuilder.Entity<WorkOrder>()
                .HasOne(a => a.WorkOrderComplete)
                .WithOne(b => b.WorkOrder)
                .HasForeignKey<WorkOrderComplete>(b => b.Id);

            modelBuilder.Entity<JobStatusUpdate>()
                .HasOne(a => a.RelatedWorkOrder)
                .WithMany(b => b.JobStatusUpdates);

            modelBuilder.Seed(_dataImporter);
        }
    }
}
