using System;
using System.Collections.Generic;

namespace RepairsApi.V2.Controllers.Parameters
{
    public class WorkOrderSearchParameters
    {
        public static int MaxPageSize { get; } = 50;
        public string PropertyReference { get; set; }
        public string ContractorReference { get; set; }
        public List<int> StatusCodes { get; set; }
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Clamp(value, 0, MaxPageSize);
        }
    }
}
