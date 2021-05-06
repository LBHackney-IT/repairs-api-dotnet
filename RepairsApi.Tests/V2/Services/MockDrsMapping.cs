using Moq;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using V2_Generated_DRS;

namespace RepairsApi.Tests.V2.Services
{
    public class MockDrsMapping : Mock<IDrsMapping>
    {
        public void SetupMappings(WorkOrder workOrder)
        {
            Setup(x => x.BuildCreateOrderRequest(It.IsAny<string>(), It.IsAny<WorkOrder>()))
                .ReturnsAsync(new createOrder
                {
                    createOrder1 = new xmbCreateOrder
                    {
                        id = workOrder.Id
                    }
                });
            Setup(x => x.BuildDeleteOrderRequest(It.IsAny<string>(), It.IsAny<WorkOrder>()))
                .ReturnsAsync(new deleteOrder()
                {
                    deleteOrder1 = new xmbDeleteOrder
                    {
                        id = workOrder.Id
                    }
                });
        }
    }
}
