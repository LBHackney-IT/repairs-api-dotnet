using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
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
        private readonly Fixture _fixture = new Fixture();

        private Mock<IListOperativesUseCase> _listOperativesUseCaseMock;
        private Mock<IGetOperativeUseCase> _getOperativeUseCaseMock;
        private Mock<IDeleteOperativeUseCase> _deleteOperativeUseCaseMock;

        private OperativesController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _listOperativesUseCaseMock = new Mock<IListOperativesUseCase>();
            _getOperativeUseCaseMock = new Mock<IGetOperativeUseCase>();
            _deleteOperativeUseCaseMock = new Mock<IDeleteOperativeUseCase>();

            _classUnderTest = new OperativesController(
                _listOperativesUseCaseMock.Object,
                _getOperativeUseCaseMock.Object,
                _deleteOperativeUseCaseMock.Object
            );
        }

        [Test]
        public async Task GetOperative()
        {
            // Arrange
            var operative = _fixture.Create<OperativeResponse>();
            _getOperativeUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(operative);

            // Act
            var objectResult = await _classUnderTest.GetOperative(operative.PayrollNumber);
            var operativesResult = GetResultData<OperativeResponse>(objectResult);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            operativesResult.Should().BeEquivalentTo(operative);
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task ListsOperatives(int operativeCount)
        {
            // Arrange
            var operatives = _fixture.CreateMany<OperativeResponse>(operativeCount);
            _listOperativesUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<OperativeRequest>()))
                .ReturnsAsync(operatives.ToList());
            var operativeSearchParams = new OperativeRequest();

            // Act
            var objectResult = await _classUnderTest.ListOperatives(operativeSearchParams);
            var operativesResult = GetResultData<List<OperativeResponse>>(objectResult);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            operativesResult.Should().HaveCount(operativeCount);
        }

        [Test]
        public async Task DeletesOperatives()
        {
            // Arrange
            const int count = 5;
            var index = new Random().Next(0, count - 1);
            var operatives = _fixture.CreateMany<OperativeResponse>(count);
            _deleteOperativeUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            var operativePrn = operatives.ElementAt(index).PayrollNumber;

            // Act
            var objectResult = await _classUnderTest.DeleteOperative(operativePrn);
            var operativePrnResult = GetResultData<string>(objectResult);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            operativePrnResult.Should().BeEquivalentTo(operativePrn);
        }

        [Test]
        public async Task DeleteReturns404IfOperativeNotFound()
        {
            // Arrange
            const int count = 5;
            var index = new Random().Next(0, count - 1);
            var operatives = _fixture.CreateMany<OperativeResponse>(count);
            _deleteOperativeUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            var operativePrn = operatives.ElementAt(index).PayrollNumber;

            // Act
            var objectResult = await _classUnderTest.DeleteOperative(operativePrn);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
        }
    }
}
