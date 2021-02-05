using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
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
            var expectedUser = new User
            {
                Name = "name", Email = "email"
            };
            _currentUserServiceMock.Setup(x => x.GetUser())
                .Returns(expectedUser);

            // act
            await _classUnderTest.CreateJobStatusUpdate(expected);

            // assert
            InMemoryDb.Instance.JobStatusUpdates
                .Should()
                .ContainSingle();
            var jsu = InMemoryDb.Instance.JobStatusUpdates.Single();
            jsu.Should().BeEquivalentTo(expected);
            jsu.Author.Should().Be(expectedUser.Name);
        }

        private JobStatusUpdate CreateJobStatusUpdate()
        {
            return _generator
                .AddValue(null, (JobStatusUpdate jsu) => jsu.Author)
                .Generate();
        }
    }
}
