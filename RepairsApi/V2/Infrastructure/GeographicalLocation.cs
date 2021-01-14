using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class GeographicalLocation
    {
        [Key] public int Id { get; set; }
        public virtual List<string> Longitude { get; set; }
        public virtual List<string> Latitude { get; set; }
        public virtual List<string> Elevation { get; set; }
        public virtual List<string> ElevationReferenceSystem { get; set; }
        public string PositionalAccuracy { get; set; }
        public virtual List<Point> Polyline { get; set; }
    }


}

