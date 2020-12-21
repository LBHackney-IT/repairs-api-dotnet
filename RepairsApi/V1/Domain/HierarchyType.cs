using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V1.Domain
{
    public class HierarchyType
    {
        public string SubTypeCode { get; internal set; }
        public string SubTypeDescription { get; internal set; }
        public string LevelCode { get; internal set; }
    }
}
