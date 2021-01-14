using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class Dependency
    {
        public DependencyTypeCode? Type { get; set; }
        public DateTimeOffset? Duration { get; set; }
    }

    public class WorkElementDependency
    {
        [Key] public int Id { get; set; }
        public virtual Dependency Dependency { get; set; }
        public virtual WorkElement DependsOnWorkElement { get; set; }
    }
}
