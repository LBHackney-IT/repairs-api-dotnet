using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
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
        private Generator<ScheduleOfRatesModel> _generator;

        [SetUp]
        public void Setup()
        {
            _generator = new Generator<ScheduleOfRatesModel>();
            _generator.AddDefaultGenerators();
            _mockScheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new ListScheduleOfRatesUseCase(_mockScheduleOfRatesGateway.Object);
        }

        [Test]
        public async Task CanExecute()
        {
            // arrange
            var expectedCodes = _generator.GenerateList(10);
            _mockScheduleOfRatesGateway.Setup(x => x.GetSorCodes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedCodes);

            // act
            var result = await _classUnderTest.Execute("trade", "property", "contractor");

            // assert
            result.Should().BeEquivalentTo(expectedCodes);
        }
    }

}
