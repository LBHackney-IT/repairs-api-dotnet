using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Helpers
{
    public static class JobStatusUpdateExtensions
    {
        public static void PrefixComments(this JobStatusUpdate jobStatusUpdate, string prefix)
        {
            if (jobStatusUpdate.Comments is null || !jobStatusUpdate.Comments.StartsWith(prefix))
            {
                jobStatusUpdate.Comments = $"{prefix}{jobStatusUpdate.Comments}";
            }
        }
    }
}
