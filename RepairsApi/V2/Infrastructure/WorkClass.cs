using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WorkClassCode = RepairsApi.V2.Generated.WorkClassCode;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class WorkClass
    {
        public string WorkClassDescription { get; set; }
        public WorkClassCode? WorkClassCode { get; set; }
        public virtual WorkClassSubType WorkClassSubType { get; set; }
    }

    [Owned]
    public class WorkClassSubType
    {
        /// <summary>
        /// Comma joined list of names
        /// </summary>
        public string WorkClassSubTypeName { get; set; }
        public string WorkClassSubTypeDescription { get; set; }
    }
}
