using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    [TestFixture]
    public class ListScheduleOfRatesUseCaseTests
    {
        private ListScheduleOfRatesUseCase _classUnderTest;
        private Mock<IScheduleOfRatesGateway> _mockScheduleOfRatesGateway;

        [SetUp]
        public void Setup()
        {
            _mockScheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new ListScheduleOfRatesUseCase(_mockScheduleOfRatesGateway.Object);
        }

        [Test]
        public async Task CanListCodes()
        {
            // arrange
            var expectedCode = new ScheduleOfRates
            {
                CustomCode = "1",
                CustomName = "name",
                SORContractorRef = "contractor",
                Priority = new SORPriority
                {
                    Description = "priorityDescription",
                    PriorityCode = 1
                }
            };
            var expectedCodes = new List<ScheduleOfRates>
            {
                expectedCode
            };
            _mockScheduleOfRatesGateway.Setup(g => g.GetSorCodes(null))
                .ReturnsAsync(expectedCodes);

            // act
            var codes = await _classUnderTest.Execute("", "");

            // assert
            codes.Single().Should().BeEquivalentTo(expectedCode.ToResponse());
        }
    }

}
