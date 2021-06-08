using System.Collections.Generic;

namespace RepairsApi.V2.Boundary.Response
{
    public class OperativeResponse
    {
        public int Id { get; set; }
        public string PayrollNumber { get; set; }
        public string Name { get; set; }
        public List<string> Trades { get; set; }
    }
}
