using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Gateways;

namespace RepairsApi.Tests.V2.Gateways
{
    public class SorPriorityGatewayTests
    {
        private SorPriorityGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new SorPriorityGateway(InMemoryDb.Instance);
        }

        [Test]
        public async Task CanListSorCodes()
        {
            InMemoryDb.Instance.SORPriorities.Add(new RepairsApi.V2.Infrastructure.Hackney.SORPriority
            {
                Description = "desc",
                PriorityCode = 1
            });
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var codes = await _classUnderTest.GetPriorities();

            // assert
            codes.Should().NotBeNullOrEmpty();
        }
    }
}
