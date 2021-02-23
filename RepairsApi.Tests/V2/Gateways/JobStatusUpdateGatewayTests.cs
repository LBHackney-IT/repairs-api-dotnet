using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using JobStatusUpdate = RepairsApi.V2.Infrastructure.JobStatusUpdate;

namespace RepairsApi.Tests.V2.Gateways
{
    public class JobStatusUpdateGatewayTests
    {
        private JobStatusUpdateGateway _classUnderTest;
        private Generator<JobStatusUpdate> _generator;
        private Mock<ICurrentUserService> _currentUserServiceMock;

        [SetUp]
        public void Setup()
        {
            SetupGenerator();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _classUnderTest = new JobStatusUpdateGateway(InMemoryDb.Instance, _currentUserServiceMock.Object);
        }

        private void SetupGenerator()
        {
            _generator = new Generator<JobStatusUpdate>()
                .AddInfrastructureWorkOrderGenerators();
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CanCreateJobStatusUpdate()
        {
            // arrange
            var expected = CreateJobStatusUpdate();
            var expectedUser = SetupUser();

            // act
            await _classUnderTest.CreateJobStatusUpdate(expected);

            // assert
            InMemoryDb.Instance.JobStatusUpdates
                .Should()
                .ContainSingle();
            var jsu = InMemoryDb.Instance.JobStatusUpdates.Single();
            jsu.Should().BeEquivalentTo(expected);
            jsu.AuthorName.Should().Be(expectedUser.Name());
            jsu.AuthorEmail.Should().Be(expectedUser.Email());
        }

        private ClaimsPrincipal SetupUser()
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.Email, "email"));
            identity.AddClaim(new Claim(ClaimTypes.Name, "name"));

            ClaimsPrincipal user = new ClaimsPrincipal(identity);
            _currentUserServiceMock.Setup(x => x.GetUser())
                .Returns(user);
            return user;
        }

        private JobStatusUpdate CreateJobStatusUpdate()
        {
            return _generator
                .AddValue(null, (JobStatusUpdate jsu) => jsu.AuthorName)
                .Generate();
        }
    }
}
