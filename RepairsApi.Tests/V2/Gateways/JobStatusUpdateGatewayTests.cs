using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    public class JobStatusUpdateGatewayTests
    {
        private JobStatusUpdateGateway _classUnderTest;
        private Generator<JobStatusUpdate> _generator;

        [SetUp]
        public void Setup()
        {
            SetupGenerator();
            _classUnderTest = new JobStatusUpdateGateway(InMemoryDb.Instance);
        }

        private void SetupGenerator()
        {
            _generator = new Generator<JobStatusUpdate>()
                .AddWorkOrderGenerators();
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

            // act
            await _classUnderTest.CreateJobStatusUpdate(expected);
            await InMemoryDb.Instance.SaveChangesAsync();

            // assert
            InMemoryDb.Instance.JobStatusUpdates
                .Should()
                .ContainSingle()
                .Which.IsSameOrEqualTo(expected);
        }

        private JobStatusUpdate CreateJobStatusUpdate()
        {
            return _generator.Generate();
        }
    }
}
