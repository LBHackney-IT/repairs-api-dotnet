using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V1.Infrastructure
{
    public class Unit
    {
        [Key] public int Id { get; set; }
        public virtual PropertyAddress Address { get; set; }
        public virtual KeySafe KeySafe { get; set; }
    }

    public class PropertyClass
    {
        [Key] public int Id { get; set; }
        public virtual ICollection<Unit> Unit { get; set; }
    }

    public class Site
    {
        [Key] public int Id { get; set; }
        public virtual ICollection<PropertyClass> PropertyRef { get; set; }
    }
}
