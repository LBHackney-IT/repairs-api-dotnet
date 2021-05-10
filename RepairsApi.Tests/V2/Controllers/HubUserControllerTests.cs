using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using RepairsApi.Tests.Helpers;

namespace RepairsApi.Tests.V2.Controllers
{
    public class HubUserControllerTests : ControllerTests
    {
        private HubUserController _classUnderTest;
        private CurrentUserServiceMock _mockCurrentUserService;

        [SetUp]
        public void SetUp()
        {
            _mockCurrentUserService = new CurrentUserServiceMock();
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

            _mockCurrentUserService.SetUser(hubUser.Sub, hubUser.Email, hubUser.Name, hubUser.VaryLimit, hubUser.RaiseLimit);

            var response = await _classUnderTest.GetHubUser() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(hubUser);
        }
    }
}
