using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepairsApi.V2.Infrastructure.Hackney;

namespace RepairsApi.V2.Infrastructure
{
    public class RepairsContext : DbContext
    {

        public RepairsContext(
            DbContextOptions<RepairsContext> options
        ) : base(options)
        {
        }

        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkElement> WorkElements { get; set; }
        public DbSet<WorkOrderComplete> WorkOrderCompletes { get; set; }
        public DbSet<JobStatusUpdate> JobStatusUpdates { get; set; }

        public DbSet<ScheduleOfRates> SORCodes { get; set; }
        public DbSet<SorCodeTrade> Trades { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<SORPriority> SORPriorities { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<PropertyContract> PropertyContracts { get; set; }
        public DbSet<SORContract> SORContracts { get; set; }

        public DbSet<AvailableAppointment> AvailableAppointments { get; set; }
        public DbSet<AvailableAppointmentDay> AvailableAppointmentDays { get; set; }
        public DbSet<Hackney.Appointment> Appointments { get; set; }
        public DbSet<SecurityGroup> SecurityGroups { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Operative> Operatives { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkOrder>()
                .Property(wo => wo.Id)
                .HasIdentityOptions(startValue: 10000000);

            modelBuilder.Entity<WorkOrder>()
                .HasOne(workOrder => workOrder.WorkOrderComplete)
                .WithOne(workOrderComplete => workOrderComplete.WorkOrder)
                .HasForeignKey<WorkOrderComplete>(b => b.Id);

            modelBuilder.Entity<JobStatusUpdate>()
                .HasOne(jobStatusUpdate => jobStatusUpdate.RelatedWorkOrder)
                .WithMany(workOrder => workOrder.JobStatusUpdates);

            modelBuilder.Entity<PropertyContract>()
                .HasKey(pc => new { pc.ContractReference, pc.PropRef });

            modelBuilder.Entity<SORContract>()
                .HasKey(pc => new { pc.ContractReference, pc.SorCodeCode });

            modelBuilder.Entity<ScheduleOfRates>()
                .HasOne<SorCodeTrade>(sor => sor.Trade)
                .WithMany()
                .HasForeignKey(sor => sor.TradeCode);

            modelBuilder.Entity<ScheduleOfRates>()
                .HasOne<SORPriority>(sor => sor.Priority)
                .WithMany()
                .HasForeignKey(sor => sor.PriorityId);

            modelBuilder.Entity<Contract>()
                .HasOne<Contractor>(c => c.Contractor)
                .WithMany(c => c.Contracts)
                .HasForeignKey(c => c.ContractorReference);

            modelBuilder.Entity<SORContract>()
                .HasOne(s => s.SorCode)
                .WithMany(sor => sor.SorCodeMap)
                .HasForeignKey(s => s.SorCodeCode);

            modelBuilder.Entity<SORContract>()
                .HasOne(s => s.Contract)
                .WithMany(c => c.SorCodeMap)
                .HasForeignKey(s => s.ContractReference);

            modelBuilder.Entity<PropertyContract>()
                .HasOne(p => p.Contract)
                .WithMany(c => c.PropertyMap)
                .HasForeignKey(p => p.ContractReference);

            modelBuilder.Entity<WorkOrder>()
                .OwnsOne(wo => wo.WorkPriority)
                    .HasOne(wp => wp.Priority)
                    .WithMany()
                    .HasForeignKey(wp => wp.PriorityCode);

            modelBuilder.Entity<SecurityGroup>()
                .HasIndex(sg => sg.GroupName)
                .IsUnique(false);

            modelBuilder.Entity<SecurityGroup>()
                .Property(sg => sg.GroupName)
                .IsRequired();

            modelBuilder.Entity<Operative>()
                .HasQueryFilter(operative => EF.Property<bool>(operative, "IsArchived") == false);
        }

        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            if (entity is IArchivable archivable)
            {
                archivable.IsArchived = true;
                return base.Update(entity);
            }

            return base.Remove(entity);
        }
    }
}
