using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkPriorityCode>().HasData(
                new WorkPriorityCode {Id = 1, Name = "Emergency"},
                new WorkPriorityCode {Id = 2, Name = "High"},
                new WorkPriorityCode {Id = 3, Name = "Medium"},
                new WorkPriorityCode {Id = 4, Name = "Low"},
                new WorkPriorityCode {Id = 5, Name = "Deferred"}
            );
        }
    }
}
