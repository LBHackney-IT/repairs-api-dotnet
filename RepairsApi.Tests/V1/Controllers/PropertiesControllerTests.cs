using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using NUnit.Framework;
using RepairsApi.V1.Boundary.Response;
using RepairsApi.V1.Controllers;
using RepairsApi.V1.Domain;
using RepairsApi.V1.UseCase;
using RepairsApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

using static RepairsApi.Tests.V1.DataFakers;

namespace RepairsApi.Tests.V1.Controllers
{
    [TestFixture]
    public class PropertiesControllerTests
    {
        private PropertiesController _classUnderTest;
        private Mock<IListAlertsUseCase> _listAlertsUseCaseMock;
        private Mock<IGetPropertyUseCase> _getPropertyUseCaseMock;
        private Mock<IListPropertiesUseCase> _listPropertiesUseCaseMock;

        [SetUp]
        public void SetUp()
        {
            _listAlertsUseCaseMock = new Mock<IListAlertsUseCase>();
            _getPropertyUseCaseMock = new Mock<IGetPropertyUseCase>();
            _listPropertiesUseCaseMock = new Mock<IListPropertiesUseCase>();
            _classUnderTest = new PropertiesController(_listAlertsUseCaseMock.Object, _getPropertyUseCaseMock.Object, _listPropertiesUseCaseMock.Object);
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task ListsProperties(int propertyCount)
        {
            // Arrange
            IEnumerable<PropertyModel> seededProperties = StubProperties().Generate(propertyCount);
            _listPropertiesUseCaseMock.Setup(m => m.ExecuteAsync(It.IsAny<PropertySearchModel>())).ReturnsAsync(seededProperties);

            // Act
            var result = await _classUnderTest.ListProperties(null, null, null).ConfigureAwait(false);
            var propertyResults = GetResultData<List<PropertyViewModel>>(result);
            var statusCode = GetStatusCode(result);

            // Assert
            statusCode.Should().Be(200);
            propertyResults.Should().HaveCount(propertyCount);
        }

        [Test]
        public async Task GetsProperties()
        {
            string expectedPropertyReference = new Faker().Random.Number().ToString();
            var property = new PropertyWithAlerts()
            {
                PropertyModel = StubProperties().Generate(),
                Alerts = StubAlerts().Generate(5)
            };
            property.PropertyModel.PropertyReference = expectedPropertyReference;

            _getPropertyUseCaseMock.Setup(m => m.ExecuteAsync(It.IsAny<string>())).ReturnsAsync(property);

            // Act
            var result = await _classUnderTest.GetProperty(expectedPropertyReference).ConfigureAwait(false);
            var propertyResult = GetResultData<PropertyResponse>(result);
            var statusCode = GetStatusCode(result);

            // Assert
            statusCode.Should().Be(200);
            propertyResult.Property.PropertyReference.Should().Be(expectedPropertyReference);
        }

        [Test]
        public async Task Returns404WhenPropertyNotFound()
        {
            string expectedPropertyReference = new Faker().Random.Number().ToString();

            _getPropertyUseCaseMock.Setup(m => m.ExecuteAsync(It.IsAny<string>())).ReturnsAsync((PropertyWithAlerts) null);

            // Act
            var result = await _classUnderTest.GetProperty(expectedPropertyReference).ConfigureAwait(false);
            var propertyResult = GetResultData<PropertyResponse>(result);
            var statusCode = GetStatusCode(result);

            // Assert
            statusCode.Should().Be(404);
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task ListsAlerts(int alertCount)
        {
            // Arrange
            string expectedPropertyReference = new Faker().Random.Number().ToString();
            PropertyAlertList alertList = StubAlertList(expectedPropertyReference, alertCount);

            _listAlertsUseCaseMock.Setup(m => m.ExecuteAsync(It.IsAny<string>())).ReturnsAsync(alertList);

            // Act
            var result = await _classUnderTest.ListCautionaryAlerts(expectedPropertyReference).ConfigureAwait(false);
            var alertResult = GetResultData<CautionaryAlertResponseList>(result);
            var statusCode = GetStatusCode(result);

            // Assert
            statusCode.Should().Be(200);
            alertResult.Alerts.Should().HaveCount(alertCount);
            alertResult.PropertyReference.Should().Be(expectedPropertyReference);
        }

        private static int? GetStatusCode(IActionResult result)
        {
            return (result as IStatusCodeActionResult).StatusCode;
        }

        private static T GetResultData<T>(IActionResult result)
            where T : class
        {
            return (result as ObjectResult)?.Value as T;
        }
    }
}
