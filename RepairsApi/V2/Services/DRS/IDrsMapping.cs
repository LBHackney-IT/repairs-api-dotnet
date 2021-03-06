using System.Threading.Tasks;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Infrastructure;
using V2_Generated_DRS;

namespace RepairsApi.V2.Services
{
    public interface IDrsMapping
    {
        Task<createOrder> BuildCreateOrderRequest(string sessionId, WorkOrder workOrder);
        Task<deleteOrder> BuildDeleteOrderRequest(string sessionId, WorkOrder workOrder);
        Task<updateBooking> BuildCompleteOrderUpdateBookingRequest(string sessionId, WorkOrder workOrder, order drsOrder);
    }
}
