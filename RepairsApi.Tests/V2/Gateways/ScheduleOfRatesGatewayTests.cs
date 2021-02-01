using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
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
            var codes = await _classUnderTest.GetSorCodes("", "");

            // assert
            codes.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task CanFilterCodesByContractor()
        {
            // arrange
            string expectedContractorRef = Guid.NewGuid().ToString();
            InMemoryDb.Instance.SORCodes.Add(new ScheduleOfRates
            {
                CustomCode = Guid.NewGuid().ToString(),
                SORContractorRef = expectedContractorRef
            });
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var codes = await _classUnderTest.GetSorCodes(expectedContractorRef);

            // assert
            codes.Should().ContainSingle().Which.SORContractorRef.Should().Be(expectedContractorRef);
        }

        [Test]
        public async Task CanGetContractorRef()
        {
            // arrange
            var sorGenerator = new Generator<ScheduleOfRates>()
                .AddGenerator(new RandomStringGenerator(10));
            var scheduleOfRatesEnumerable = sorGenerator.GenerateList(25);
            await InMemoryDb.Instance.SORCodes.AddRangeAsync(scheduleOfRatesEnumerable);
            var expectedSorCode = new ScheduleOfRates
            {
                CustomCode = Guid.NewGuid().ToString(),
                SORContractorRef = Guid.NewGuid().ToString()
            };
            await InMemoryDb.Instance.SORCodes.AddAsync(expectedSorCode);
            await InMemoryDb.Instance.SaveChangesAsync();


            // act
            var result = await _classUnderTest.GetContractorReference(expectedSorCode.CustomCode);

            // assert
            result.Should().Be(expectedSorCode.SORContractorRef);
        }
    }
}
