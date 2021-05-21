using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.UseCase.Interfaces;
using OperativeSearchParams = RepairsApi.V2.Boundary.Request.Operative;

namespace RepairsApi.Tests.V2.Controllers
{
    [TestFixture]
    public class OperativesControllerTests : ControllerTests
    {
        private OperativesController _classUnderTest;
        private Mock<IListOperativesUseCase> _listOperativesUseCaseMock;

        [SetUp]
        public void SetUp()
        {
            _listOperativesUseCaseMock = new Mock<IListOperativesUseCase>();
            _classUnderTest = new OperativesController(_listOperativesUseCaseMock.Object);
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task ListsOperatives(int operativeCount)
        {
            // Arrange
            var operatives = new Faker<Operative>().Generate(operativeCount);
            _listOperativesUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<OperativeSearchParams>()))
                .ReturnsAsync(operatives);
            var operativeSearchParams = new OperativeSearchParams();

            // Act
            var objectResult = await _classUnderTest.ListOperatives(operativeSearchParams);
            var operativesResult = GetResultData<List<Operative>>(objectResult);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            operativesResult.Should().HaveCount(operativeCount);
        }
    }
}
