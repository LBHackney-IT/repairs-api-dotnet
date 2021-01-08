using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{
    [Owned]
    public class WorkClass
    {
        [Column("work_class_code")] public int WorkClassCode { get; set; }
    }
}
