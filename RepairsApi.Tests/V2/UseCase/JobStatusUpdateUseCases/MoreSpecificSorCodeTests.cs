using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2;
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
        public async Task ThrowWhenWorkOrderNotInCorrectState(
            [Values(WorkStatusCode.VariationPendingApproval, WorkStatusCode.PendingApproval)]
            WorkStatusCode state)
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = state;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var request = BuildUpdate(workOrder);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            (await fn.Should().ThrowAsync<InvalidOperationException>())
                .Which.Message.Should().Be(Resources.ActionUnsupported);
        }


        [Test]
        public async Task SetsVariationPendingApprovalWhenAuthRequired()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Open;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var request = BuildUpdate(workOrder);
            _featureManagerMock.Setup(x => x.IsEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            _authorisationMock.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed);

            await _classUnderTest.Execute(request);

            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationPendingApproval);
            request.TypeCode.Should().Be(Generated.JobStatusUpdateTypeCode._180);
        }


        [TestCase(true, false)] // feature on, below limit
        [TestCase(false, false)] // feature off, below limit
        [TestCase(false, true)] // feature off, above limit
        public async Task CallsUpdateSorCodeUseCaseWhenAuthNotRequired(bool featureEnabled, bool authorisationRequired)
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Open;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var request = BuildUpdate(workOrder);
            _featureManagerMock.Setup(x => x.IsEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(featureEnabled);
            _authorisationMock.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(authorisationRequired ? AuthorizationResult.Failed() : AuthorizationResult.Success());

            await _classUnderTest.Execute(request);

            _updateSorCodesUseCaseMock.Verify(x => x.Execute(workOrder, It.IsAny<WorkElement>()));
        }

        [Test]
        public async Task PrependsRejectString()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Open;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var jobStatusUpdate = BuildUpdate(workOrder);
            const string beforeComments = "expectedBeforeComments";
            jobStatusUpdate.Comments = beforeComments;

            await _classUnderTest.Execute(jobStatusUpdate);

            jobStatusUpdate.Comments.Should().Contain(beforeComments);
            jobStatusUpdate.Comments.Should().Contain(Resources.VariationReason);
        }

        [Test]
        public async Task DoesntPrependsRejectStringWhenAlreadyPresent()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Open;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var jobStatusUpdate = BuildUpdate(workOrder);
            var expectedComments = $"{Resources.VariationReason}expectedBeforeComments";
            jobStatusUpdate.Comments = expectedComments;

            await _classUnderTest.Execute(jobStatusUpdate);

            jobStatusUpdate.Comments.Should().Be(expectedComments);
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

        private Generated.JobStatusUpdate BuildUpdate(WorkOrder workOrder)
        {

            return _fixture.Build<Generated.JobStatusUpdate>()
                .With(jsu => jsu.MoreSpecificSORCode, _fixture.Build<Generated.WorkElement>()
                    .With(we => we.RateScheduleItem, _fixture.Build<Generated.RateScheduleItem>()
                        .With(rsi => rsi.Quantity, _fixture.Build<Generated.Quantity>()
                            .With(q => q.Amount, _fixture.CreateMany<double>(1).ToArray)
                            .Create())
                        .CreateMany().ToArray)
                    .Create())
                .With(jsu => jsu.RelatedWorkOrderReference, _fixture.Build<Generated.Reference>()
                    .With(r => r.ID, workOrder.Id.ToString)
                    .Create())
                .Create();
        }
    }
}
