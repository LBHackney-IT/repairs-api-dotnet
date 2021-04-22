using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Services;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Services
{
    public class CurrentUserServiceTests
    {
        private Mock<IGroupsGateway> _groupGatewayMock;
        private CurrentUserService _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _groupGatewayMock = new Mock<IGroupsGateway>();
            _classUnderTest = new CurrentUserService(new NullLogger<CurrentUserService>(), _groupGatewayMock.Object);
        }

        [Test]
        public async Task SetsUser()
        {
            const string Jwt = TestUserInformation.Jwt;
            await _classUnderTest.LoadUser(Jwt);

            var user = _classUnderTest.GetUser();

            user.Should().NotBeNull();
            user.Sub().Should().Be(TestUserInformation.Sub);
            user.Name().Should().Be(TestUserInformation.Name);
            user.Email().Should().Be(TestUserInformation.Email);
            user.RaiseLimit().Should().Be("0");
            user.VaryLimit().Should().Be("0");
        }

        [Test]
        public async Task NullUserFromBadJWT()
        {
            const string JWT = "NotAValidJWT";
            await _classUnderTest.LoadUser(JWT);

            var user = _classUnderTest.GetUser();

            user.Should().BeNull();
        }
    }
}
