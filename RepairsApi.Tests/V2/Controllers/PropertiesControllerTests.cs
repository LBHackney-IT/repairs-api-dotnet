using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Domain;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

using static RepairsApi.Tests.V2.DataFakers;

namespace RepairsApi.Tests.V2.Controllers
{
    [TestFixture]
    public class PropertiesControllerTests : ControllerTests
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
            _classUnderTest = new PropertiesController(_listAlertsUseCaseMock.Object,
                _getPropertyUseCaseMock.Object,
                _listPropertiesUseCaseMock.Object,
                new NullLogger<PropertiesController>());
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task ListsProperties(int propertyCount)
        {
            // Arrange
            IEnumerable<PropertyModel> seededProperties = StubProperties().Generate(propertyCount);
            _listPropertiesUseCaseMock.Setup(m => m.ExecuteAsync(It.IsAny<PropertySearchModel>())).ReturnsAsync(seededProperties);

            // Act
            var result = await _classUnderTest.ListProperties(null, null, null);
            var propertyResults = GetResultData<List<PropertyViewModel>>(result);
            var statusCode = GetStatusCode(result);

            // Assert
            statusCode.Should().Be(200);
            propertyResults.Should().HaveCount(propertyCount);
        }

        [Test]
        public async Task GetsProperty()
        {
            string expectedPropertyReference = new Faker().Random.Number().ToString();
            var property = new PropertyWithAlerts()
            {
                PropertyModel = StubProperties().Generate(),
                PersonAlerts = StubAlerts().Generate(5),
                LocationAlerts = StubAlerts().Generate(5)
            };
            property.PropertyModel.PropertyReference = expectedPropertyReference;

            _getPropertyUseCaseMock.Setup(m => m.ExecuteAsync(It.IsAny<string>())).ReturnsAsync(property);

            // Act
            var result = await _classUnderTest.GetProperty(expectedPropertyReference);
            var propertyResult = GetResultData<PropertyResponse>(result);
            var statusCode = GetStatusCode(result);

            // Assert
            statusCode.Should().Be(200);
            propertyResult.Property.PropertyReference.Should().Be(expectedPropertyReference);
        }

        [TestCase(0, 0)]
        [TestCase(5, 0)]
        [TestCase(0, 6)]
        [TestCase(8, 7)]
        public async Task ListsAlerts(int propertyAlertCount, int personAlertCount)
        {
            // Arrange
            string expectedPropertyReference = new Faker().Random.Number().ToString();
            AlertList alertList = new AlertList
            {
                PropertyAlerts = StubPropertyAlertList(expectedPropertyReference, propertyAlertCount),
                PersonAlerts = new PersonAlertList { Alerts = StubAlerts().Generate(personAlertCount) }
            };

            _listAlertsUseCaseMock.Setup(m => m.ExecuteAsync(It.IsAny<string>())).ReturnsAsync(alertList);

            // Act
            var result = await _classUnderTest.ListCautionaryAlerts(expectedPropertyReference);
            var alertResult = GetResultData<CautionaryAlertResponseList>(result);
            var statusCode = GetStatusCode(result);

            // Assert
            statusCode.Should().Be(200);
            alertResult.LocationAlert.Should().HaveCount(propertyAlertCount);
            alertResult.PersonAlert.Should().HaveCount(personAlertCount);
            alertResult.PropertyReference.Should().Be(expectedPropertyReference);
        }
    }
}
