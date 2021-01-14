using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class AdditionalWork
    {
        [Key] public int Id { get; set; }
        public virtual WorkOrder AdditionalWorkOrder { get; set; }
        public string ReasonRequired { get; set; }
    }


}

