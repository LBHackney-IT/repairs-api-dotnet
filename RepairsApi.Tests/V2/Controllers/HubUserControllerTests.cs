using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Services;

namespace RepairsApi.Tests.V2.Controllers
{
    public class HubUserControllerTests : ControllerTests
    {
        private HubUserController _classUnderTest;
        private Mock<ICurrentUserService> _currentUserService;

        [SetUp]
        public void SetUp()
        {
            _currentUserService = new Mock<ICurrentUserService>();
            _classUnderTest = new HubUserController(_currentUserService.Object);
        }

        [Test]
        [Ignore("wip")]
        public async Task GetHubUser()
        {
            // Arrange
            // Act
            var response = await _classUnderTest.GetHubUser();
            //var contractors = GetResultData<List<Contractor>>(response);
            var statusCode = GetStatusCode(response);

            // Assert
            statusCode.Should().Be(200);
            //contractors.Should().HaveCount(contractorCount);
        }
    }
}
