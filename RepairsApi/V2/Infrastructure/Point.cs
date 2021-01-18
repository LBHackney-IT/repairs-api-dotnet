using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Point
    {
        [Key] public int Id { get; set; }
        public int Sequence { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
    }
}

