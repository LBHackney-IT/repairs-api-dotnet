using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Services;

namespace RepairsApi.Tests.V2.Services
{
    public class CurrentUserServiceTests
    {
        private CurrentUserService _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new CurrentUserService(new NullLogger<CurrentUserService>());
        }

        [Test]
        public void SetsUser()
        {
            const string JWT = TestUserInformation.JWT;
            _classUnderTest.LoadUser(JWT);

            var user = _classUnderTest.GetUser();

            user.Should().NotBeNull();
            user.Name().Should().Be(TestUserInformation.NAME);
            user.Email().Should().Be(TestUserInformation.EMAIL);
        }

        [Test]
        public void NullUserFromBadJWT()
        {
            const string JWT = "NotAValidJWT";
            _classUnderTest.LoadUser(JWT);

            var user = _classUnderTest.GetUser();

            user.Should().BeNull();
        }
    }
}
