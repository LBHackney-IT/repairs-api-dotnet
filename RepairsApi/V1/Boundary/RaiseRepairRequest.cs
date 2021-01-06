using System.Collections.Generic;
using System.Linq;
using RepairsApi.V1.Controllers;
using RepairsApi.V1.Domain.Repair;

namespace RepairsApi.V1.Boundary
{
    public class RaiseRepairRequest
    {
        public string Description { get; set; }
        public int Priority { get; set; }
        public List<RepairCode> RepairCodes { get; set; }

        internal WorkOrder ToDomain()
        {
            var raiseRepair = new WorkOrder
            {
                DescriptionOfWork = Description,
                WorkPriority = new WorkPriority
                {
                    PriorityCode = Priority.ToString()
                },
                WorkElements = RepairCodes.Select(rc => new WorkElement
                {
                    RateScheduleItems = new List<RateScheduleItem>
                    {
                        new RateScheduleItem
                        {
                            CustomCode = rc.Code.ToString(),
                            Quantity = new Quantity
                            {
                                Amount = rc.Quantity
                            }
                        }
                    }
                }).ToList()
            };


            return raiseRepair;
        }
    }
}
