using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
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
        public virtual PropertyAddress Address { get; set; }
        public string MasterKeySystem { get; set; }
        public string PropertyReference { get; set; }
    }

    public class Site
    {
        [Key] public int Id { get; set; }
        public virtual ICollection<PropertyClass> PropertyClass { get; set; }
        public string Name { get; set; }
    }
}
