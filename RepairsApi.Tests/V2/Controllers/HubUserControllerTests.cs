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

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Email, "repairs@hackney.gov.uk"));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, "222222"));
            identity.AddClaim(new Claim(ClaimTypes.Name, "Bob Repairs"));
            identity.AddClaim(new Claim(CustomClaimTypes.VARYLIMIT, "250"));
            identity.AddClaim(new Claim(CustomClaimTypes.RAISELIMIT, "250"));
            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            _mockCurrentUserService.Setup(x => x.GetUser())
                .Returns(user);
            var response = await _classUnderTest.GetHubUser() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(hubUser);
        }
    }
}
