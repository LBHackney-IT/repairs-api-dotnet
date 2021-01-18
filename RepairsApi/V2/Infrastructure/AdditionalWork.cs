using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class AdditionalWork
    {
        public virtual WorkOrder AdditionalWorkOrder { get; set; }
        public string ReasonRequired { get; set; }
    }
}

