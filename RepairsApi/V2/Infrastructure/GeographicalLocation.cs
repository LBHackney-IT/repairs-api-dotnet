using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class GeographicalLocation
    {
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? Elevation { get; set; }
        public string ElevationReferenceSystem { get; set; }
        public string PositionalAccuracy { get; set; }
        public virtual List<Point> Polyline { get; set; }
    }
}

