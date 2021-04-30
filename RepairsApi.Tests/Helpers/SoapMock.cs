using System;
using Moq;
using V2_Generated_DRS;

namespace RepairsApi.Tests.Helpers
{
    public class SoapMock : Mock<SOAP>
    {
        public string ExpectedToken { get; set; } = Guid.NewGuid().ToString();

        public SoapMock()
        {
            Setup(x => x.openSessionAsync(It.IsAny<openSession>()))
                .ReturnsAsync(new openSessionResponse { @return = new xmbOpenSessionResponse { status = responseStatus.success } });
            Setup(x => x.deleteOrderAsync(It.IsAny<deleteOrder>()))
                .ReturnsAsync(new deleteOrderResponse { @return = new xmbDeleteOrderResponse { status = responseStatus.success } });
            Setup(x => x.createOrderAsync(It.IsAny<createOrder>()))
                .ReturnsAsync(new createOrderResponse
                {
                    @return = new xmbCreateOrderResponse
                    {
                        status = responseStatus.success,
                        theOrder = new order
                        {
                            theBookings = new[] { new booking { tokenId = ExpectedToken } }
                        }
                    }
                });
        }
    }
}
