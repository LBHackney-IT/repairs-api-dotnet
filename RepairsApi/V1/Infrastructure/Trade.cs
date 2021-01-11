using System.ComponentModel.DataAnnotations;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    public class Trade
    {
        [Key] public int Id { get; set; }
        public TradeCode Code { get; set; }
        public string CustomCode { get; set; }
        public string CustomName { get; set; }
    }
}
