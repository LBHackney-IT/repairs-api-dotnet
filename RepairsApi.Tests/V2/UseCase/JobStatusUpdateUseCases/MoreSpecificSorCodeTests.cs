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
using RepairsApi.V2;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using Generated = RepairsApi.V2.Generated;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class MoreSpecificSorCodeTests
    {
        private Fixture _fixture;

        private AuthorisationMock _authorisationMock;
        private CurrentUserServiceMock _currentUserServiceMock;
        private Mock<IUpdateSorCodesUseCase> _updateSorCodesUseCaseMock;
        private Mock<IScheduleOfRatesGateway> _sheduleOfRatesGateway;
        private FeatureManagerMock _featureManagerMock;
        private NotificationMock _notifierMock;
        private MoreSpecificSorUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _authorisationMock = new AuthorisationMock();
            _featureManagerMock = new FeatureManagerMock();
            _currentUserServiceMock = new CurrentUserServiceMock();
            _updateSorCodesUseCaseMock = new Mock<IUpdateSorCodesUseCase>();
            _sheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _sheduleOfRatesGateway.Setup(g => g.GetCost(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(10.0);
            _notifierMock = new NotificationMock();
            _classUnderTest = new MoreSpecificSorUseCase(
                _authorisationMock.Object,
                _featureManagerMock.Object,
                _currentUserServiceMock.Object,
                _updateSorCodesUseCaseMock.Object,
                _sheduleOfRatesGateway.Object,
                _notifierMock);
        }

        [TestCase(WorkStatusCode.VariationPendingApproval)]
        [TestCase(WorkStatusCode.PendingApproval)]
        public async Task ThrowWhenWorkOrderNotInCorrectState(WorkStatusCode state)
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = state;
            var request = BuildUpdate(workOrder);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            (await fn.Should().ThrowAsync<NotSupportedException>())
                .Which.Message.Should().Be(Resources.ActionUnsupported);
        }


        [Test]
        public async Task SetsVariationPendingApprovalWhenAuthRequired()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Open;
            var request = BuildUpdate(workOrder);
            _featureManagerMock.SetFeature(FeatureFlags.SpendLimits, true);
            _authorisationMock.SetPolicyResult("VarySpendLimit", false);

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
            var request = BuildUpdate(workOrder);
            _featureManagerMock.SetFeature(FeatureFlags.SpendLimits, featureEnabled);

            _authorisationMock.SetPolicyResult("VarySpendLimit", !authorisationRequired);

            await _classUnderTest.Execute(request);

            _updateSorCodesUseCaseMock.Verify(x => x.Execute(workOrder, It.IsAny<WorkElement>()));
        }

        [Test]
        public async Task PrependsRejectString()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Open;
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
            var jobStatusUpdate = BuildUpdate(workOrder);
            var expectedComments = $"{Resources.VariationReason}expectedBeforeComments";
            jobStatusUpdate.Comments = expectedComments;

            await _classUnderTest.Execute(jobStatusUpdate);

            jobStatusUpdate.Comments.Should().Be(expectedComments);
        }

        [TestCase(10.0)]
        public async Task CostIsAttachedToUpdates(double cost)
        {
            const int desiredWorkOrderId = 42;
            _sheduleOfRatesGateway.Setup(g => g.GetCost(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(cost);
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Open;
            var jobStatusUpdate = BuildUpdate(workOrder);

            await _classUnderTest.Execute(jobStatusUpdate);

            foreach (var rsi in jobStatusUpdate.MoreSpecificSORCode.RateScheduleItem)
            {
                rsi.CodeCost.Should().Be(cost);
            }
        }

        [Test]
        public async Task HighCostNotificationSent()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Open;
            var request = BuildUpdate(workOrder);
            _featureManagerMock.SetFeature(FeatureFlags.SpendLimits, true);
            _authorisationMock.SetPolicyResult("VarySpendLimit", false);

            await _classUnderTest.Execute(request);

            _notifierMock.HaveHandlersBeenCalled<HighCostVariationCreated>().Should().BeTrue();
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

        private JobStatusUpdate BuildUpdate(WorkOrder workOrder)
        {

            return _fixture.Build<JobStatusUpdate>()
                .With(jsu => jsu.MoreSpecificSORCode, _fixture.Build<WorkElement>()
                    .With(we => we.RateScheduleItem, _fixture.Build<RateScheduleItem>()
                        .With(rsi => rsi.Quantity, _fixture.Build<Quantity>()
                            .With(q => q.Amount, 1)
                            .Create())
                        .CreateMany().ToList())
                    .Create())
                .With(jsu => jsu.RelatedWorkOrder, workOrder)
                .Create();
        }
    }
}
