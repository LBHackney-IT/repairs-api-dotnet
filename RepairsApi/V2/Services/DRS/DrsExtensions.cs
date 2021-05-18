using System.Linq;
using Castle.Core.Internal;
using V2_Generated_DRS;

namespace RepairsApi.V2.Services.DRS
{
    public static class DrsExtensions
    {
        public static bool IsScheduled(this order order)
        {
            return order.theBookings.All(b => !b.theResources.IsNullOrEmpty());
        }
    }
}
