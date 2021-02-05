using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
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
            const string JWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMDA1MTg4ODg3NDY5MjIxMTY2NDciLCJlbWFpbCI6ImhhY2tuZXkudXNlckB0ZXN0LmhhY2tuZXkuZ292LnVrIiwiaXNzIjoiSGFja25leSIsIm5hbWUiOiJIYWNrbmV5IFVzZXIiLCJncm91cHMiOlsiZ3JvdXAgMSIsImdyb3VwIDIiXSwiaWF0IjoxNTcwNDYyNzMyfQ.BxBlEHWHGU6GkPO5DZoshJp3VQcrm2umaMkQ7Q7zxw8";
            _classUnderTest.LoadUser(JWT);

            var user = _classUnderTest.GetUser();

            user.Should().NotBeNull();
            user.Name.Should().Be("Hackney User");
            user.Email.Should().Be("hackney.user@test.hackney.gov.uk");
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
