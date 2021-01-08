using System;
using System.ComponentModel.DataAnnotations;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    public class Dependency
    {
        [Key] public int Id { get; set; }
        public DependencyTypeCode Type { get; set; }
        public DateTimeOffset Duration { get; set; }
    }

    public class WorkElementDependency
    {
        [Key] public int Id { get; set; }
        public virtual Dependency Dependency { get; set; }
        public virtual WorkElement DependsOnWorkElement { get; set; }
    }
}
