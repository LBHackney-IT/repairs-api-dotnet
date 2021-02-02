using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Exceptions;
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
            _classUnderTest = new MoreSpecificSorUseCase(_repairsGatewayMock.Object, InMemoryDb.Instance);
        }

        [Test]
        public async Task MoreSpecificSORCodeAddsSORCodeToWorkOrder()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var expectedNewCodes = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem).ToList();
            expectedNewCodes.Add(new RateScheduleItem
            {
                CustomCode = "code",
                Quantity = new Quantity
                {
                    Amount = 4.5
                }
            });
            var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCodes.ToArray());

            await _classUnderTest.Execute(request);

            workOrder.WorkElements.Single().RateScheduleItem.Should().BeEquivalentTo(expectedNewCodes, options => options.Excluding(rsi => rsi.Id));
        }

        [Test]
        public async Task UpdateQuantityOfExistingCodes()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var codeToModify = workOrder.WorkElements.First().RateScheduleItem.First();
            var expectedNewCode = CloneRateScheduleItem(codeToModify);
            expectedNewCode.Quantity.Amount += 4;
            var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCode);

            await _classUnderTest.Execute(request);

            codeToModify.Should().BeEquivalentTo(expectedNewCode, option => option.Excluding(x => x.Id));
        }

        [Test]
        public void ThrowUnsupportedWhenOriginalSorCodeNotPResent()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var expectedNewCode = new RateScheduleItem
            {
                CustomCode = "code",
                Quantity = new Quantity
                {
                    Amount = 4.5
                }
            };
            var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCode);

            Assert.ThrowsAsync<NotSupportedException>(() => _classUnderTest.Execute(request));
        }

        [Test]
        public void ThrowWhenWorkOrderNotFound()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            var expectedNewCode = new RateScheduleItem
            {
                CustomCode = "code",
                Quantity = new Quantity
                {
                    Amount = 4.5
                },
                ContractReference = "contractReference"
            };
            var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCode);

            Assert.ThrowsAsync<ResourceNotFoundException>(() => _classUnderTest.Execute(request));
        }

        private static RateScheduleItem CloneRateScheduleItem(RateScheduleItem toModify)
        {

            var expectedNewCodes = new RateScheduleItem
            {
                CustomCode = toModify.CustomCode,
                CustomName = toModify.CustomName,
                Quantity = new Quantity
                {
                    Amount = toModify.Quantity.Amount,
                    UnitOfMeasurementCode = toModify.Quantity.UnitOfMeasurementCode
                },
                CodeCost = toModify.CodeCost,
                DateCreated = toModify.DateCreated,
                M3NHFSORCode = toModify.M3NHFSORCode,
                ContractReference = toModify.ContractReference
            };
            return expectedNewCodes;
        }

        private static Generated.JobStatusUpdate CreateMoreSpecificSORUpdateRequest(int desiredWorkOrderId, WorkOrder workOrder, params RateScheduleItem[] expectedNewCodes)
        {
            var newCodes = expectedNewCodes.Select(rsi => new Generated.RateScheduleItem
            {
                CustomCode = rsi.CustomCode,
                Quantity = rsi.Quantity.ToResponse(),
                CustomName = rsi.CustomName,
                M3NHFSORCode = rsi.M3NHFSORCode,
                ContractReference = rsi.ContractReference
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

        private WorkOrder CreateReturnWorkOrder(int expectedId)
        {
            var workOrder = BuildWorkOrder(expectedId);

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == expectedId)))
                .ReturnsAsync(workOrder);
            _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == expectedId)))
                .ReturnsAsync(_fixture.Create<List<WorkElement>>);

            return workOrder;
        }

        private WorkOrder BuildWorkOrder(int expectedId)
        {

            var workOrder = _fixture.Build<WorkOrder>()
                .With(x => x.WorkElements, new List<WorkElement>
                {
                    _fixture.Build<WorkElement>()
                        .With(x => x.RateScheduleItem,
                            new List<RateScheduleItem>
                            {
                                _fixture.Create<RateScheduleItem>()
                            }
                        ).With(x => x.Trade,
                            new List<Trade>
                            {
                                _fixture.Create<Trade>()
                            })
                        .Create()
                })
                .With(x => x.Id, expectedId)
                .Create();
            return workOrder;
        }
    }
}