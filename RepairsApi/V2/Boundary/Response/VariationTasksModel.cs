using System.Collections.Generic;

namespace RepairsApi.V2.Boundary.Response
{
    public class GetVariationResponse
    {
        public string Notes { get; set; }
        public IEnumerable<VariationTasksModel> Tasks { get; set; }
    }

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
