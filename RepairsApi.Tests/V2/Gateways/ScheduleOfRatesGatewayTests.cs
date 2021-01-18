using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    public class ScheduleOfRatesGatewayTests
    {
        private ScheduleOfRatesGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new ScheduleOfRatesGateway(InMemoryDb.Instance);
        }

        [Test]
        public async Task CanListSorCodes()
        {
            // act
            var codes = await _classUnderTest.GetSorCodes();

            // assert
            codes.Should().NotBeNullOrEmpty();
        }
    }
}
