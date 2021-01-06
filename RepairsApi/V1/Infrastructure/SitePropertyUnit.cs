using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{
    [Owned]
    public class SitePropertyUnit
    {
        [Column("reference")] public string Reference { get; set; }
    }
}
