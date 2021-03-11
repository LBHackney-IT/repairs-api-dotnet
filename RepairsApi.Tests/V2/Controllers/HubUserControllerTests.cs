using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RepairsApi.Tests.V2.Controllers
{
    public class HubUserControllerTests : ControllerTests
    {
        private HubUserController _classUnderTest;
        private Mock<ICurrentUserService> _mockCurrentUserService;

        [SetUp]
        public void SetUp()
        {
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _classUnderTest = new HubUserController(_mockCurrentUserService.Object);
        }

        [Test]
        public async Task GetHubUser()
        {
            // Arrange
            var hubUser = new HubUserModel
            {
                Sub = "222222",
                Email = "repairs@hackney.gov.uk",
                Name = "Bob Repairs",
                VaryLimit = "250",
                RaiseLimit = "250"
            };

            _mockCurrentUserService.Setup(x => x.GetHubUser()).Returns(hubUser);
            var response = await _classUnderTest.GetHubUser() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(hubUser);
        }
    }
}
