using Hackney.Housing.Hact;
using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{
    [Owned]
    public class Quantity
    {
        public double Amount { get; set; }
        public string UnitOfMeasurementCode { get; set; }
    }
}
