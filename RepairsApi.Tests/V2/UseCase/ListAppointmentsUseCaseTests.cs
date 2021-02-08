using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    public class ListAppointmentsUseCaseTests
    {
        private Mock<IRepairsGateway> _repairGatewayMock;
        private ListAppointmentsUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _repairGatewayMock = new Mock<IRepairsGateway>();
            _classUnderTest = new ListAppointmentsUseCase(_repairGatewayMock.Object);
        }

        [Test]
        public async Task ThrowsIfNoWorkOrder()
        {
            _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync((WorkOrder)null);
            int workOrder = 0;
            Func<Task> testFn = async () => await _classUnderTest.Execute(workOrder, DateTime.UtcNow, DateTime.UtcNow);

            await testFn.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test]
        public async Task ThrowsIfNoWorkOrder()
        {
            _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync((WorkOrder) null);
            int workOrder = 0;
            Func<Task> testFn = async () => await _classUnderTest.Execute(workOrder, DateTime.UtcNow, DateTime.UtcNow);

            await testFn.Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}
