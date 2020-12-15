using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase
{
    public class PropertySearchModel
    {
        public string Address { get; internal set; }
        public string PostCode { get; internal set; }
        public string Query { get; internal set; }
    }
}
