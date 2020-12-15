using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{

    public class RepairsContext : DbContext
    {
        public RepairsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RateScheduleItem> RateScheduleItems { get; set; }
        public DbSet<WorkElement> WorkElements { get; set; }

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
        }
    }
}
