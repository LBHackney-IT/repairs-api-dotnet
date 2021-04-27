using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class Quantity
    {
        public Quantity() { }

        public Quantity(int amount)
        {
            Amount = amount;
        }

        public double Amount { get; set; }
        public UNECEUnitOfMeasurementCodeC0? UnitOfMeasurementCode { get; set; }
    }
}
