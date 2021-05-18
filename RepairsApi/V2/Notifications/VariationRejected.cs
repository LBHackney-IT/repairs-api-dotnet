using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Notifications
{
    public class VariationRejected : INotification
    {
        public JobStatusUpdate Variation { get; }
        public JobStatusUpdate Rejection { get; }

        public VariationRejected(JobStatusUpdate variation, JobStatusUpdate rejection)
        {
            Variation = variation;
            Rejection = rejection;
        }
    }
}
