using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure.Hackney;

namespace RepairsApi.V2.Infrastructure
{
    public class RepairsContext : DbContext
    {

        public RepairsContext(
            DbContextOptions options
        ) : base(options)
        {
        }

        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkElement> WorkElements { get; set; }
        public DbSet<WorkOrderComplete> WorkOrderCompletes { get; set; }
        public DbSet<JobStatusUpdate> JobStatusUpdates { get; set; }

        public DbSet<ScheduleOfRates> SORCodes { get; set; }
        public DbSet<SorCodeTrade> Trades { get; }
        public DbSet<Contractor> Contractors { get; }
        public DbSet<SORPriority> SORPriorities { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<PropertyContract> PropertyContracts { get; set; }
        public DbSet<SORContract> SORContracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkOrder>()
                .Property(wo => wo.Id)
                .HasIdentityOptions(startValue: 10000000);

            modelBuilder.Entity<WorkOrder>()
                .HasOne(a => a.WorkOrderComplete)
                .WithOne(b => b.WorkOrder)
                .HasForeignKey<WorkOrderComplete>(b => b.Id);

            modelBuilder.Entity<PropertyContract>()
                .HasKey(pc => new { pc.ContractReference, pc.PropRef });

            modelBuilder.Entity<SORContract>()
                .HasKey(pc => new { pc.ContractReference, pc.SorCodeCode });

        }
    }
}
