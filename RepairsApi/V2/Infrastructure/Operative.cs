using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure.Hackney;

namespace RepairsApi.V2.Infrastructure
{
    public class Operative : IArchivable
    {
        [Key] public int Id { get; set; }
        public string PayrollNumber { get; set; }
        public bool IsArchived { get; set; }
        public string Name { get; set; }
        [Required] public string ResourceId { get; set; }
        public virtual List<SorCodeTrade> Trades { get; set; }
        public virtual List<WorkOrderOperative> WorkOrderOperatives { get; set; }
        public virtual List<WorkOrder> AssignedWorkOrders { get; set; }
    }
}
