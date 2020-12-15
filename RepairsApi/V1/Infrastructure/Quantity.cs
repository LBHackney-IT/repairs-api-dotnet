using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{
    [Owned]
    public class Quantity
    {
        public int Amount { get; set; }
        public string UnitOfMeasurementCode { get; set; }
    }
}
