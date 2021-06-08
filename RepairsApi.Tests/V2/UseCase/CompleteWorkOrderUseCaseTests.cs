using FluentAssertions;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Generated = RepairsApi.V2.Generated;

namespace RepairsApi.Tests.V2.UseCase
{
    public class CompleteWorkOrderUseCaseTests
    {
        private Mock<IRepairsGateway> _repairsGatewayMock;
        private CurrentUserServiceMock _currentUserServiceMock;
        private CompleteWorkOrderUseCase _classUnderTest;
        private Generator<WorkOrder> _generator;
        private MockWorkOrderCompletionGateway _workOrderCompletionGatewayMock;
        private NotificationMock _handlerMock;
        private Mock<IFeatureManager> _featureManager;

        [SetUp]
        public void Setup()
        {
            _generator = CreateGenerator();
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _currentUserServiceMock = new CurrentUserServiceMock();
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Agent, true);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Contractor, true);
            _workOrderCompletionGatewayMock = new MockWorkOrderCompletionGateway();
            _handlerMock = new NotificationMock();
            _featureManager = new Mock<IFeatureManager>();
            OperativeRequired(false);
            _classUnderTest = new CompleteWorkOrderUseCase(
                _repairsGatewayMock.Object,
                _workOrderCompletionGatewayMock.Object,
                InMemoryDb.TransactionManager,
                _currentUserServiceMock.Object,
                _handlerMock,
                _featureManager.Object);
        }

        private static Generator<WorkOrder> CreateGenerator()
        {
            return new Generator<WorkOrder>()
                .AddWorkOrderGenerators();
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task ThrowsNotSupportedWhenNot1Update(int updateCount)
        {
            var expectedWorkOrder = CreateWorkOrder();

            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>();

            for (int i = 0; i < updateCount; i++)
            {
                workOrderCompleteRequest.JobStatusUpdates.Add(
                                    new Generated.JobStatusUpdates
                                    {
                                        TypeCode = Generated.JobStatusUpdateTypeCode._0,
                                        OtherType = CustomJobStatusUpdates.Completed,
                                        Comments = "expectedComment"
                                    });
            }

            Func<Task> testFn = () => _classUnderTest.Execute(workOrderCompleteRequest);

            await testFn.Should().ThrowAsync<NotSupportedException>();
        }

        public async Task CanCompleteWorkOrder()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Contractor, true);
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = CustomJobStatusUpdates.Completed, Comments = "expectedComment"
                }
            };

            // act
            await _classUnderTest.Execute(workOrderCompleteRequest);

            var lastWorkOrderComplete = _workOrderCompletionGatewayMock.LastWorkOrderComplete;
            lastWorkOrderComplete.Should().NotBeNull();
            lastWorkOrderComplete.JobStatusUpdates.Should().HaveCount(workOrderCompleteRequest.JobStatusUpdates.Count);
        }

        [Test]
        public async Task ThrowsWhenAlreadyComplete()
        {
            // arrange
            _workOrderCompletionGatewayMock.Setup(m => m.IsWorkOrderCompleted(It.IsAny<int>())).ReturnsAsync(true);

            // act
            Func<Task> fn = async () => await _classUnderTest.Execute(CreateRequest(1));

            // assert
            await fn.Should().ThrowAsync<NotSupportedException>().WithMessage(Resources.CannotCompleteWorkOrderTwice);
        }

        [Test]
        public async Task ThrowsWhenNotAssignedWhenFeatureOn()
        {
            // arrange
            OperativeRequired(true);
            var generator = CreateGenerator()
                .AddValue(null, (WorkOrder wo) => wo.AssignedOperatives);
            var expectedWorkOrder = CreateWorkOrder(generator);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Contractor, true);
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = CustomJobStatusUpdates.Completed, Comments = "expectedComment"
                }
            };

            // act
            Func<Task> fn = async () => await _classUnderTest.Execute(workOrderCompleteRequest);

            // assert
            await fn.Should().ThrowAsync<NotSupportedException>().WithMessage(Resources.CannotCompleteWithNoOperative);
        }

        [Test]
        public void ThrowsExceptionWhenRelatedWorkElementPresent()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0,
                    OtherType = "expectedOtherType",
                    Comments = "expectedComment",
                    RelatedWorkElementReference = new List<Generated.Reference>
                    {
                        new Generated.Reference()
                    }
                }
            };

            // act
            // assert
            Assert.ThrowsAsync<NotSupportedException>(() => _classUnderTest.Execute(workOrderCompleteRequest));
        }

        [Test]
        public void ThrowsExceptionWhenAdditionalWorkPresent()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0,
                    OtherType = CustomJobStatusUpdates.Completed,
                    Comments = "expectedComment",
                    AdditionalWork = new Generated.AdditionalWork()
                }
            };

            // act
            // assert
            Assert.ThrowsAsync<NotSupportedException>(() => _classUnderTest.Execute(workOrderCompleteRequest));
        }


        [TestCase(Generated.JobStatusUpdateTypeCode._0, "expectedOtherType")]
        [TestCase(Generated.JobStatusUpdateTypeCode._10, "")]
        public async Task ThrowsExceptionWhenNotValidUpdateType(Generated.JobStatusUpdateTypeCode typeCode, string otherTypeCode)
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = typeCode, OtherType = otherTypeCode, Comments = "expectedComment"
                }
            };

            // act
            // assert
            Func<Task> act = () => _classUnderTest.Execute(workOrderCompleteRequest);

            (await act.Should().ThrowAsync<NotSupportedException>())
                .Which.Message.Should().Be(Resources.UnsupportedWorkOrderUpdate);
        }

        [Test]
        public void ThrowsExceptionWhenFollowOnWorkPresent()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.FollowOnWorkOrderReference = new List<Generated.Reference>
            {
                new Generated.Reference()
            };

            // act
            // assert
            Assert.ThrowsAsync<NotSupportedException>(() => _classUnderTest.Execute(workOrderCompleteRequest));
        }

        [Test]
        public async Task UpdatesWorkOrderStatusCancelled()
        {
            // arrange
            string customUpdateType = CustomJobStatusUpdates.Cancelled;
            WorkStatusCode expectedNewStatus = WorkStatusCode.Canceled;
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = customUpdateType, Comments = "expectedComment"
                }
            };

            // act
            await _classUnderTest.Execute(workOrderCompleteRequest);

            // assert
            _repairsGatewayMock.Verify(rgm => rgm.UpdateWorkOrderStatus(expectedWorkOrder.Id, expectedNewStatus));
        }

        [Test]
        public async Task ContractorUpdatesWorkOrderStatusComplete()
        {
            // arrange
            string customUpdateType = CustomJobStatusUpdates.Completed;
            WorkStatusCode expectedNewStatus = WorkStatusCode.Complete;
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Contractor, true);
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = customUpdateType, Comments = "expectedComment"
                }
            };

            // act
            await _classUnderTest.Execute(workOrderCompleteRequest);

            // assert
            _repairsGatewayMock.Verify(rgm => rgm.UpdateWorkOrderStatus(expectedWorkOrder.Id, expectedNewStatus));
        }

        [Test]
        public async Task ContractorUpdatesWorkOrderStatusNoAccess()
        {
            // arrange
            const Generated.JobStatusUpdateTypeCode NoAccess = Generated.JobStatusUpdateTypeCode._70;
            WorkStatusCode expectedNewStatus = WorkStatusCode.NoAccess;
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Contractor, true);
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = NoAccess, Comments = "expectedComment"
                }
            };

            // act
            await _classUnderTest.Execute(workOrderCompleteRequest);

            // assert
            _repairsGatewayMock.Verify(rgm => rgm.UpdateWorkOrderStatus(expectedWorkOrder.Id, expectedNewStatus));
        }

        [Test]
        public async Task ContractManagerUpdatesWorkOrderStatusComplete()
        {
            // arrange
            string customUpdateType = CustomJobStatusUpdates.Completed;
            WorkStatusCode expectedNewStatus = WorkStatusCode.Complete;
            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager, true);
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = customUpdateType, Comments = "expectedComment"
                }
            };

            // act
            await _classUnderTest.Execute(workOrderCompleteRequest);

            // assert
            _repairsGatewayMock.Verify(rgm => rgm.UpdateWorkOrderStatus(expectedWorkOrder.Id, expectedNewStatus));
        }

        [Test]
        public async Task ContractManagerUpdatesWorkOrderStatusNoAccess()
        {
            // arrange
            const Generated.JobStatusUpdateTypeCode NoAccess = Generated.JobStatusUpdateTypeCode._70;
            WorkStatusCode expectedNewStatus = WorkStatusCode.NoAccess;
            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager, true);
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = NoAccess, Comments = "expectedComment"
                }
            };

            // act
            await _classUnderTest.Execute(workOrderCompleteRequest);

            // assert
            _repairsGatewayMock.Verify(rgm => rgm.UpdateWorkOrderStatus(expectedWorkOrder.Id, expectedNewStatus));
        }

        [Test]
        public async Task HandlersCalledWhenCompleted()
        {
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = CustomJobStatusUpdates.Completed, Comments = "expectedComment"
                }
            };

            await _classUnderTest.Execute(workOrderCompleteRequest);

            _handlerMock.HaveHandlersBeenCalled<WorkOrderCompleted>().Should().BeTrue();
        }

        [Test]
        public async Task HandlersCalledWhenCancelled()
        {
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = CustomJobStatusUpdates.Cancelled, Comments = "expectedComment"
                }
            };

            await _classUnderTest.Execute(workOrderCompleteRequest);

            _handlerMock.HaveHandlersBeenCalled<WorkOrderCancelled>().Should().BeTrue();
        }

        [TestCase(Generated.JobStatusUpdateTypeCode._70, "")]
        [TestCase(Generated.JobStatusUpdateTypeCode._0, CustomJobStatusUpdates.Completed)]
        [TestCase(Generated.JobStatusUpdateTypeCode._0, CustomJobStatusUpdates.Cancelled)]
        public async Task ThrowsUnauthorizedWhenNotAuthorized(Generated.JobStatusUpdateTypeCode updateCode, string otherTypeCode)
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Contractor, false);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager, false);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Agent, false);

            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = updateCode, OtherType = otherTypeCode, Comments = "expectedComment"
                }
            };

            // act
            Func<Task> act = () => _classUnderTest.Execute(workOrderCompleteRequest);

            (await act.Should().ThrowAsync<UnauthorizedAccessException>())
                .Which.Message.Should().BeOneOf(new List<string>
                {
                    Resources.NotAuthorisedToCancel, Resources.NotAuthorisedToClose
                });
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task CanCancelPendingOrder(bool featureFlagOn)
        {
            // arrange
            OperativeRequired(featureFlagOn);
            var generator = CreateGenerator()
                .AddValue(null, (WorkOrder wo) => wo.AssignedOperatives);
            var expectedWorkOrder = CreateWorkOrder(generator);
            expectedWorkOrder.StatusCode = WorkStatusCode.PendingApproval;
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = CustomJobStatusUpdates.Cancelled, Comments = "expectedComment"
                }
            };

            // act
            await _classUnderTest.Execute(workOrderCompleteRequest);

            _repairsGatewayMock.Verify(rgm => rgm.UpdateWorkOrderStatus(expectedWorkOrder.Id, WorkStatusCode.Canceled));
        }

        private static Generated.WorkOrderComplete CreateRequest(int expectedWorkOrderId)
        {
            var request = new Generated.WorkOrderComplete
            {
                WorkOrderReference = new Generated.Reference
                {
                    ID = expectedWorkOrderId.ToString()
                }
            };
            return request;
        }

        private WorkOrder CreateWorkOrder()
        {
            return CreateWorkOrder(_generator);
        }

        private WorkOrder CreateWorkOrder(Generator<WorkOrder> generator)
        {
            var expectedWorkOrder = generator?.Generate();
            _repairsGatewayMock.Setup(r => r.GetWorkOrder(expectedWorkOrder.Id))
                .ReturnsAsync(expectedWorkOrder);
            return expectedWorkOrder;
        }

        private void OperativeRequired(bool enabled)
        {
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.EnforceAssignedOperative))
                .ReturnsAsync(enabled);
        }
    }

    public class MockWorkOrderCompletionGateway : Mock<IWorkOrderCompletionGateway>
    {
        public WorkOrderComplete LastWorkOrderComplete { get; set; }

        public MockWorkOrderCompletionGateway()
        {
            Setup(g => g.CreateWorkOrderCompletion(It.IsAny<WorkOrderComplete>()))
                .ReturnsAsync(1)
                .Callback<WorkOrderComplete>(woc => LastWorkOrderComplete = woc);
        }
    }
}
