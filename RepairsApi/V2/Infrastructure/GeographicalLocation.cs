using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class GeographicalLocation
    {
        [Key] public int Id { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? Elevation { get; set; }
        public string ElevationReferenceSystem { get; set; }
        public string PositionalAccuracy { get; set; }
        public virtual List<Point> Polyline { get; set; }
    }
}

