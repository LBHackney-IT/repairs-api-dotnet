using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using Generated = RepairsApi.V2.Generated;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class MoreSpecificSorCodeTests
    {
        private Fixture _fixture;

        private MockRepairsGateway _repairsGatewayMock;
        private MoreSpecificSorUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repairsGatewayMock = new MockRepairsGateway();
            _classUnderTest = new MoreSpecificSorUseCase(_repairsGatewayMock.Object);
        }

        [Test]
        public async Task MoreSpecificSORCodeAddsSORCodeToWorkOrder()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateWorkOrder(desiredWorkOrderId);
            var expectedNewCodes = workOrder.WorkElements.First().RateScheduleItem.Select(rsi => rsi.CustomCode).ToList();
            expectedNewCodes.Add("newCode");
            var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCodes.ToArray());

            await _classUnderTest.Execute(request);

            _repairsGatewayMock.Verify(g => g.AddWorkElement(desiredWorkOrderId, It.IsAny<WorkElement>()));
            _repairsGatewayMock.LastWorkElement.Should().BeEquivalentTo(request.MoreSpecificSORCode.ToDb());
        }

        [Test]
        public async Task ThrowUnsupportedWhenOriginalSorCodeNotPResent()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateWorkOrder(desiredWorkOrderId);
            const string expectedNewCode = "newCode";
            var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCode);

            await _classUnderTest.Execute(request);

            Assert.ThrowsAsync<NotSupportedException>(() => _classUnderTest.Execute(request));
        }

        private static Generated.JobStatusUpdate CreateMoreSpecificSORUpdateRequest(int desiredWorkOrderId, WorkOrder workOrder, params string[] expectedNewCodes)
        {
            var newCodes = expectedNewCodes.Select(c => new Generated.RateScheduleItem
            {
                CustomCode = c
            });

            return new Generated.JobStatusUpdate
            {
                RelatedWorkOrderReference = new Generated.Reference
                {
                    ID = desiredWorkOrderId.ToString()
                },
                TypeCode = Generated.JobStatusUpdateTypeCode._80,
                MoreSpecificSORCode = new Generated.WorkElement
                {
                    Trade = new List<Generated.Trade>
                    {
                        workOrder.WorkElements.First().Trade.First().ToResponse()
                    },
                    RateScheduleItem = newCodes.ToList()
                }
            };
        }

        private WorkOrder CreateWorkOrder(int expectedId)
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.Id = expectedId;

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == expectedId)))
                .ReturnsAsync(workOrder);
            _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == expectedId)))
                .ReturnsAsync(_fixture.Create<List<WorkElement>>);

            return workOrder;
        }
    }
}
