using RepairsApi.V2.Enums;

namespace RepairsApi.V1.Domain
{
    public class SORPriority
    {

        public WorkPriorityCode PriorityCode
        {
            get; set;
        }

        public string Description
        {
            get { return PriorityCode.ToString(); }
        }
    }
}
