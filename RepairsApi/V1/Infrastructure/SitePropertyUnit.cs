using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{
    public class SitePropertyUnit
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("reference")] public string Reference { get; set; }
        public virtual Address Address { get; set; }
    }

}
