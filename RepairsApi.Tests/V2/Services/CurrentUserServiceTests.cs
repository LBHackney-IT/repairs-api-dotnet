using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Services;
using System.Collections.Generic;

namespace RepairsApi.Tests.V2.Services
{
    public class CurrentUserServiceTests
    {
        private CurrentUserService _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            GroupOptions options = new GroupOptions
            {
                SecurityGroups = new Dictionary<string, PermissionsModel>(),
                RaiseLimitGroups = new Dictionary<string, SpendLimitModel>(),
                VaryLimitGroups = new Dictionary<string, SpendLimitModel>()
            };
            _classUnderTest = new CurrentUserService(new NullLogger<CurrentUserService>(), Options.Create(options));
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
