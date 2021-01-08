using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    [Owned]
    public class Address
    {
        [Column("address_postbox")]public string Postbox { get; set; }
        [Column("address_room")]public string Room { get; set; }
        [Column("address_department")]public string Department { get; set; }
        [Column("address_floor")]public string Floor { get; set; }
        [Column("address_plot")]public string Plot { get; set; }
        [Column("address_building_number")]public string BuildingNumber { get; set; }
        [Column("address_building_name")]public string BuildingName { get; set; }
        [Column("address_complex_name")]public string ComplexName { get; set; }
        [Column("address_street_name")]public string StreetName { get; set; }
        [Column("address_city_name")]public string CityName { get; set; }
        [Column("address_country")]public CountryCode Country { get; set; }
        [Column("address_line")]public string AddressLine { get; set; }
        [Column("address_type")]public string Type { get; set; }
        [Column("address_postal_code")]public string PostalCode { get; set; }
    }
}
