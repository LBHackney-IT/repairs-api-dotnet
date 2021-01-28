using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
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
    public class ListWorkOrderTasksUseCaseTest
    {
        private Mock<IRepairsGateway> _repairsGatewayMock;
        private ListWorkOrderTasksUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _classUnderTest = new ListWorkOrderTasksUseCase(_repairsGatewayMock.Object);
        }

        [Test]
        public async Task FlattensData()
        {
            IEnumerable<WorkElement> expected = new Generator<WorkElement>().GenerateList(10);
            _repairsGatewayMock.Setup(m => m.GetWorkElementsForWorkOrder(It.IsAny<int>())).ReturnsAsync(expected);

            var result = await _classUnderTest.Execute(1);

            int expectedCount = expected.Aggregate(0, (count, we) => count + we.RateScheduleItem.Count);
            result.Should().HaveCount(expectedCount);
        }

        [Test]
        public async Task ThrowWhenNotFound()
        {
            _repairsGatewayMock.Setup(m => m.GetWorkElementsForWorkOrder(It.IsAny<int>())).ReturnsAsync((IEnumerable<WorkElement>) null);

            Func<Task> sutFunc = async () => await _classUnderTest.Execute(1);

            await sutFunc.Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}
