using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class EmailHandler : INotificationHandler<WorkOrderCompleted>
    {
        public EmailHandler()
        {

        }

        public Task Notify(WorkOrderCompleted data)
        {
            return Task.CompletedTask;
        }
    }
}
