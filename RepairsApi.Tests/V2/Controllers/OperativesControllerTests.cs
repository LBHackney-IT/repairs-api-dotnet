using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Request;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.UseCase.Interfaces;

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
            var operatives = new Faker<OperativeResponse>().Generate(operativeCount);
            _listOperativesUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<OperativeRequest>()))
                .ReturnsAsync(operatives);
            var operativeSearchParams = new OperativeRequest();

            // Act
            var objectResult = await _classUnderTest.ListOperatives(operativeSearchParams);
            var operativesResult = GetResultData<List<OperativeResponse>>(objectResult);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            operativesResult.Should().HaveCount(operativeCount);
        }
    }
}
