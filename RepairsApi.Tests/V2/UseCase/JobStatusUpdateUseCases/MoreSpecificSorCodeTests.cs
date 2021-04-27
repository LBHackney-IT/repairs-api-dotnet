using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
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
        private Mock<IAuthorizationService> _authorisationMock;
        private CurrentUserServiceMock _currentUserServiceMock;
        private Mock<IUpdateSorCodesUseCase> _updateSorCodesUseCaseMock;
        private Mock<IFeatureManager> _featureManagerMock;
        private MoreSpecificSorUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repairsGatewayMock = new MockRepairsGateway();
            _authorisationMock = new Mock<IAuthorizationService>();
            _featureManagerMock = new Mock<IFeatureManager>();
            _authorisationMock = new Mock<IAuthorizationService>();
            _currentUserServiceMock = new CurrentUserServiceMock();
            _updateSorCodesUseCaseMock = new Mock<IUpdateSorCodesUseCase>();
            _classUnderTest = new MoreSpecificSorUseCase(
                _repairsGatewayMock.Object,
                _authorisationMock.Object,
                _featureManagerMock.Object,
                _currentUserServiceMock.Object,
                _updateSorCodesUseCaseMock.Object);
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
                }
            };
            var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCode);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            fn.Should().ThrowAsync<ResourceNotFoundException>();
        }

        private static Generated.JobStatusUpdate CreateMoreSpecificSORUpdateRequest(int desiredWorkOrderId, WorkOrder workOrder, params RateScheduleItem[] expectedNewCodes)
        {
            var newCodes = expectedNewCodes.Select(rsi => new Generated.RateScheduleItem
            {
                CustomCode = rsi.CustomCode,
                Quantity = rsi.Quantity.ToResponse(),
                CustomName = rsi.CustomName,
                M3NHFSORCode = rsi.M3NHFSORCode,
                Id = rsi.Id.ToString()
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
