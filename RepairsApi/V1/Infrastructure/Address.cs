using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    public class Address
    {
        [Key] public int Id { get; set; }
        public string Postbox { get; set; }
        public string Room { get; set; }
        public string Department { get; set; }
        public string Floor { get; set; }
        public string Plot { get; set; }
        public string BuildingNumber { get; set; }
        public string BuildingName { get; set; }
        public string ComplexName { get; set; }
        public string StreetName { get; set; }
        public string CityName { get; set; }
        public CountryCode Country { get; set; }
        public string AddressLine { get; set; }
        public string Type { get; set; }
        public string PostalCode { get; set; }
    }

    public class PostalAddress
    {
        [Key] public int Id { get; set; }
        public virtual Address Address { get; set; }
    }

    public class PropertyAddress
    {
        [Key] public int Id { get; set; }
        public virtual PostalAddress Address { get; set; }
    }
}
