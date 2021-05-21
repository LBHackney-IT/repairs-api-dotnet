using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class VariationApproved : INotification
    {
        public JobStatusUpdate Variation { get; }
        public JobStatusUpdate Approval { get; }

        public VariationApproved(JobStatusUpdate variation, JobStatusUpdate approval)
        {
            Variation = variation;
            Approval = approval;
        }
    }
}
