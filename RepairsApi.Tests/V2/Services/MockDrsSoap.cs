using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RepairsApi.V2.Services;
using V2_Generated_DRS;

namespace RepairsApi.Tests.V2.Services
{
    internal class MockDrsSoap : Mock<SOAP>
    {
        private readonly IOptions<DrsOptions> _drsOptions;
        public openSession LastOpen { get; private set; }
        public string SessionId { get; }

        public MockDrsSoap(IOptions<DrsOptions> drsOptions)
        {
            _drsOptions = drsOptions;
            SessionId = Guid.NewGuid().ToString();
            Setup(x => x.openSessionAsync(It.IsAny<openSession>()))
                .Callback<openSession>(o => LastOpen = o)
                .ReturnsAsync(new openSessionResponse
                {
                    @return = new xmbOpenSessionResponse
                    {
                        sessionId = SessionId,
                        status = responseStatus.success
                    }
                });
        }

        public void CreateReturns(responseStatus status, string errorMsg = null)
        {
            Setup(x => x.createOrderAsync(It.IsAny<createOrder>()))
                .ReturnsAsync(new createOrderResponse
                {
                    @return = new xmbCreateOrderResponse
                    {
                        status = status,
                        errorMsg = errorMsg
                    }
                });
        }

        public void DeleteReturns(responseStatus status, string errorMsg = null)
        {
            Setup(x => x.deleteOrderAsync(It.IsAny<deleteOrder>()))
                .ReturnsAsync(new deleteOrderResponse
                {
                    @return = new xmbDeleteOrderResponse
                    {
                        status = status,
                        errorMsg = errorMsg
                    }
                });
        }

        public void UpdateBookingReturns(responseStatus status, string errorMsg = null)
        {
            Setup(x => x.updateBookingAsync(It.IsAny<updateBooking>()))
                .ReturnsAsync(new updateBookingResponse
                {
                    @return = new xmbUpdateBookingResponse
                    {
                        status = status,
                        errorMsg = errorMsg
                    }
                });
        }

        public void SelectOrderReturns(order order, responseStatus status = responseStatus.success, string error = "")
        {
            Setup(x => x.selectOrderAsync(It.IsAny<selectOrder>()))
                .ReturnsAsync(new selectOrderResponse
                {
                    @return = new xmbSelectOrderResponse
                    {
                        status = status,
                        errorMsg = error,
                        theOrders = new[]
                        {
                            order
                        }
                    }
                });
        }

        public void VerifyOpenSession()
        {
            LastOpen.openSession1.login.Should().Be(_drsOptions.Value.Login);
            LastOpen.openSession1.password.Should().Be(_drsOptions.Value.Password);
        }
    }
}
