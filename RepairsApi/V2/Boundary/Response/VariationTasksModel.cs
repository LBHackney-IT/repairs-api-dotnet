using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Boundary.Response
{
    public class VariationTasksModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public double? UnitCost { get; set; }
        public double? OriginalQuantity { get; set; }
        public double? CurrentQuantity { get; set; }
        public double? VariedQuantity { get; set; }
    }
}
