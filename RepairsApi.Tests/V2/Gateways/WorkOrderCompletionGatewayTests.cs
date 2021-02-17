using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;

namespace RepairsApi.Tests.V2.Gateways
{
    public class WorkOrderCompletionGatewayTests
    {
        private WorkOrderCompletionGateway _classUnderTest;
        private Generator<WorkOrderComplete> _generator;
        private Mock<ICurrentUserService> _currentUserServiceMock;

        [SetUp]
        public void Setup()
        {
            SetupGenerator();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _classUnderTest = new WorkOrderCompletionGateway(InMemoryDb.Instance, _currentUserServiceMock.Object);
        }

        private void SetupGenerator()
        {
            _generator = new Generator<WorkOrderComplete>()
                .AddInfrastructureWorkOrderGenerators();
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CanCreateWorkOrderCompletion()
        {
            // arrange
            SetupUser();
            var expected = CreateWorkOrderCompletion();

            // act
            await _classUnderTest.CreateWorkOrderCompletion(expected);
            await InMemoryDb.Instance.SaveChangesAsync();

            // assert
            InMemoryDb.Instance.WorkOrderCompletes.Should().ContainSingle().Which.IsSameOrEqualTo(expected);
        }

        [Test]
        public async Task CreateSetsUserInfo()
        {
            // arrange
            var expected = CreateWorkOrderCompletion();
            var expectedUser = SetupUser();

            // act
            await _classUnderTest.CreateWorkOrderCompletion(expected);
            await InMemoryDb.Instance.SaveChangesAsync();

            // assert
            InMemoryDb.Instance.WorkOrderCompletes.Should().ContainSingle().Which.JobStatusUpdates.All(jsu =>
                jsu.AuthorName == expectedUser.Name &&
                jsu.AuthorEmail == expectedUser.Email
            ).Should().BeTrue();
        }

        private User SetupUser()
        {
            var expectedUser = new User
            {
                Name = "name",
                Email = "email"
            };
            _currentUserServiceMock.Setup(x => x.GetUser())
                .Returns(expectedUser);
            return expectedUser;
        }

        [Test]
        public async Task IsCompleteReturnsTrueForCompletedWorkOrder()
        {
            var workOrderId = await AddWorkOrderCompletion();

            var isComplete = await _classUnderTest.IsWorkOrderCompleted(workOrderId);

            isComplete.Should().BeTrue();
        }


        [Test]
        public async Task IsCompleteReturnsFalseForNotCompletedWorkOrder()
        {
            var expectedWorkOrder = CreateWorkOrderCompletion().WorkOrder;

            var entry = await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            var isComplete = await _classUnderTest.IsWorkOrderCompleted(entry.Entity.Id);

            isComplete.Should().BeFalse();
        }


        private async Task<int> AddWorkOrderCompletion()
        {
            var completion = CreateWorkOrderCompletion();

            var entry = await InMemoryDb.Instance.WorkOrderCompletes.AddAsync(completion);
            await InMemoryDb.Instance.SaveChangesAsync();

            return entry.Entity.WorkOrder.Id;
        }

        private WorkOrderComplete CreateWorkOrderCompletion()
        {
            return _generator.Generate();
        }
    }
}
