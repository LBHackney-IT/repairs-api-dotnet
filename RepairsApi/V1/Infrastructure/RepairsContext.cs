using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{

    public class RepairsContext : DbContext
    {
        public RepairsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<AlertRegardingPerson> PersonAlerts { get; set; }
        public DbSet<AlertRegardingLocation> LocationAlerts { get; set; }
        public DbSet<RateScheduleItem> RateScheduleItems { get; set; }
        public DbSet<WorkElement> WorkElements { get; set; }
        public DbSet<WorkPriority> WorkPriorities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RateScheduleItem>().OwnsOne(
                rateScheduleItem => rateScheduleItem.Quantity,
                nav =>
                {
                    nav.Property(quantity => quantity.Amount)
                        .HasColumnName("amount");
                    nav.Property(quantity => quantity.UnitOfMeasurementCode)
                        .HasColumnName("unit_of_measurement_code");
                });

            modelBuilder.Entity<WorkOrder>()
                .Property(wo => wo.Id)
                .HasIdentityOptions(startValue: 10000000);

            modelBuilder.Seed();
        }
    }
}
