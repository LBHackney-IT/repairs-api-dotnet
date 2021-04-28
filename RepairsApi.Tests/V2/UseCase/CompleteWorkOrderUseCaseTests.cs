using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Enums;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using Generated = RepairsApi.V2.Generated;
using RepairsApi.V2.UseCase;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Services;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Notifications;

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

        [SetUp]
        public void Setup()
        {
            ConfigureGenerator();
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _currentUserServiceMock = new CurrentUserServiceMock();
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Agent, true);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Contractor, true);
            _workOrderCompletionGatewayMock = new MockWorkOrderCompletionGateway();
            _handlerMock = new NotificationMock();
            _classUnderTest = new CompleteWorkOrderUseCase(
                _repairsGatewayMock.Object,
                _workOrderCompletionGatewayMock.Object,
                InMemoryDb.TransactionManager,
                _currentUserServiceMock.Object,
                _handlerMock);
        }

        private void ConfigureGenerator()
        {
            _generator = new Generator<WorkOrder>()
                .AddWorkOrderGenerators();
        }

        [Test]
        public async Task CanCompleteWorkOrder()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);

            // act
            await _classUnderTest.Execute(workOrderCompleteRequest);

            var lastWorkOrderComplete = _workOrderCompletionGatewayMock.LastWorkOrderComplete;
            lastWorkOrderComplete.Should().NotBeNull();
            lastWorkOrderComplete.WorkOrder.Should().BeEquivalentTo(expectedWorkOrder);
        }

        [Test]
        public async Task CanCompleteWorkOrderWithJobStatusUpdates()
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
        public async Task ReturnFalseWhenAlreadyComplete()
        {
            // arrange
            _workOrderCompletionGatewayMock.Setup(m => m.IsWorkOrderCompleted(It.IsAny<int>())).ReturnsAsync(true);

            // act
            Func<Task> fn = async () => await _classUnderTest.Execute(CreateRequest(1));

            // assert
            await fn.Should().ThrowAsync<NotSupportedException>();
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


        [Test]
        public void ThrowsExceptionWhenNotValidUpdateType()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = "expectedOtherType", Comments = "expectedComment"
                }
            };

            // act
            // assert
            Assert.ThrowsAsync<NotSupportedException>(() => _classUnderTest.Execute(workOrderCompleteRequest));
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

            var expectedWorkOrder = _generator.Generate();
            _repairsGatewayMock.Setup(r => r.GetWorkOrder(expectedWorkOrder.Id))
                .ReturnsAsync(expectedWorkOrder);
            return expectedWorkOrder;
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
