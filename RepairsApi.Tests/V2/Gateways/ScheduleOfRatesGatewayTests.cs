using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;

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

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CanGetTrades()
        {
            // arrange
            var generator = new Generator<SorCodeTrade>();
            generator.AddDefaultGenerators();
            var expectedTrades = generator.GenerateList(10);
            await InMemoryDb.Instance.Trades.AddRangeAsync(expectedTrades);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var result = await _classUnderTest.GetTrades();

            // assert
            result.Should().BeEquivalentTo(expectedTrades);
        }

        [Test]
        public async Task CanGetContractsByContractor()
        {
            // arrange
            var expectedContractor = new Contractor
            {
                Name = "name",
                Reference = "reference"
            };
            var generator = new Generator<Contract>();
            generator
                .AddDefaultGenerators()
                .AddValue(expectedContractor, (Contract c) => c.Contractor)
                .AddValue(null, (Contract c) => c.PropertyMap)
                .AddValue(null, (Contract c) => c.SorCodeMap);

            var expectedContracts = generator.GenerateList(10);
            await InMemoryDb.Instance.Contractors.AddAsync(expectedContractor);
            await InMemoryDb.Instance.Contracts.AddRangeAsync(expectedContracts);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var result = await _classUnderTest.GetContracts(expectedContractor.Reference);

            // assert
            result.Should().BeEquivalentTo(expectedContracts.Select(c => c.ContractReference));
        }

        [Test]
        public async Task CanGetCosts()
        {
            // arrange
            var generator = new Generator<SORContract>();
            Contract contract = new Contract
            {
                ContractorReference = "contractor",
                ContractReference = "contract"
            };
            generator
                .AddDefaultGenerators()
                .AddValue(null, (SORContract c) => c.SorCode)
                .AddValue(contract, (SORContract c) => c.Contract);
            var expectedContract = generator.Generate();

            await InMemoryDb.Instance.SORContracts.AddAsync(expectedContract);
            await InMemoryDb.Instance.SORContracts.AddRangeAsync(generator.GenerateList(10));
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var result = await _classUnderTest.GetCost(contract.ContractorReference, expectedContract.SorCodeCode);

            // assert
            result.Should().Be(expectedContract.Cost);
        }

        [Test]
        public async Task ValidatesContractDates()
        {
            // arrange
            const string expectedProperty = "property";
            var expectedPriority = await SeedPriority();
            var expectedTrade = await SeedTrade(Guid.NewGuid().ToString());
            string contractorReference = "contractor";
            await SeedContractor(contractorReference);
            var invalidContracts = await SeedContracts(expectedProperty, DateTime.UtcNow.AddDays(-14), DateTime.UtcNow.AddDays(-7), contractorReference);
            var validContracts = await SeedContracts(expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7), contractorReference);

            await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade, invalidContracts.First());
            var expectedCodes = await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade, validContracts.First());

            await InMemoryDb.Instance.SaveChangesAsync();

            await GetAndValidateCodes(expectedProperty, expectedTrade, contractorReference, expectedCodes);
        }

        [Test]
        public async Task ValidatesPropertyRef()
        {
            // arrange
            const string expectedProperty = "property";
            var expectedPriority = await SeedPriority();
            string contractorReference = "contractor";
            await SeedContractor(contractorReference);
            var expectedTrade = await SeedTrade(Guid.NewGuid().ToString());
            var invalidContracts = await SeedContracts("not" + expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7), contractorReference);
            var validContracts = await SeedContracts(expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7), contractorReference);

            await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade, invalidContracts.First());
            var expectedCodes = await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade, validContracts.First());

            await InMemoryDb.Instance.SaveChangesAsync();

            await GetAndValidateCodes(expectedProperty, expectedTrade, contractorReference, expectedCodes);
        }

        [Test]
        public async Task ValidatesTradeCode()
        {
            // arrange
            const string expectedProperty = "property";
            var expectedPriority = await SeedPriority();
            string contractorReference = "contractor";
            await SeedContractor(contractorReference);
            var expectedTrade = await SeedTrade(Guid.NewGuid().ToString());
            var notExpectedTrade = await SeedTrade(Guid.NewGuid().ToString());
            var validContracts = await SeedContracts(expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7), contractorReference);

            await SeedSorCodes(expectedPriority, expectedProperty, notExpectedTrade, validContracts.First());
            var expectedCodes = await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade, validContracts.First());

            await InMemoryDb.Instance.SaveChangesAsync();

            await GetAndValidateCodes(expectedProperty, expectedTrade, contractorReference, expectedCodes);
        }

        private static async Task SeedContractor(string contractorReference)
        {
            InMemoryDb.Instance.Contractors.Add(new Contractor
            {
                Reference = contractorReference,
                Name = "contractor"
            });
            await InMemoryDb.Instance.SaveChangesAsync();
        }

        private async Task GetAndValidateCodes(string expectedProperty, SorCodeTrade expectedTrade, string contractorReference, List<ScheduleOfRates> expectedCodes)
        {

            // act
            var result = await _classUnderTest.GetSorCodes(expectedProperty, expectedTrade.Code, contractorReference);

            // assert
            var expectedResult = expectedCodes.Select(sor => new ScheduleOfRatesModel
            {
                CustomCode = sor.CustomCode,
                CustomName = sor.CustomName,
                Priority = new RepairsApi.V2.Domain.SORPriority
                {
                    PriorityCode = sor.Priority?.PriorityCode,
                    Description = sor.Priority?.Description,
                }
            }).ToList();
            result.Should().BeEquivalentTo(expectedResult);
        }

        private static async Task<List<ScheduleOfRates>> SeedSorCodes(SORPriority expectedPriority, string expectedProperty, SorCodeTrade expectedTrade, Contract expectedContract = null)
        {
            var expectedGenerator = new Generator<ScheduleOfRates>();

            expectedGenerator
                .AddDefaultGenerators()
                .AddValue(expectedPriority, (ScheduleOfRates sor) => sor.Priority)
                .AddValue(expectedProperty, (PropertyContract pc) => pc.PropRef)
                .AddValue(expectedTrade, (ScheduleOfRates sor) => sor.Trade)
                .AddGenerator(() => generateJoinEntry(expectedContract), (ScheduleOfRates sor) => sor.SorCodeMap);
            var expectedCodes = expectedGenerator.GenerateList(10);

            await InMemoryDb.Instance.SORCodes.AddRangeAsync(expectedCodes);
            return expectedCodes;
        }

        private static List<SORContract> generateJoinEntry(Contract expectedContract)
        {
            return new List<SORContract>
            {
                new SORContract
                {
                    Contract = expectedContract
                }
            };
        }

        private static async Task<List<Contract>> SeedContracts(string expectedProperty, DateTime effectiveDate, DateTime termDate, string contractorReference)
        {

            var contractGenerator = new Generator<Contract>()
                .AddDefaultGenerators()
                .AddValue(contractorReference, (Contract c) => c.ContractorReference)
                .Ignore((Contract c) => c.Contractor)
                .Ignore((Contract c) => c.PropertyMap)
                .AddValue(null, (Contract c) => c.SorCodeMap)
                .AddValue(effectiveDate, (Contract c) => c.EffectiveDate)
                .AddValue(termDate, (Contract c) => c.TerminationDate);

            var contracts = contractGenerator.GenerateList(10);

            var propMaps = contracts.Select(c => new PropertyContract
            {
                PropRef = expectedProperty,
                ContractReference = c.ContractReference
            });

            await InMemoryDb.Instance.Contracts.AddRangeAsync(contracts);
            await InMemoryDb.Instance.PropertyContracts.AddRangeAsync(propMaps);

            return contracts;
        }

        private static async Task<SorCodeTrade> SeedTrade(string expectedTradeCode)
        {
            var expectedTrade = new SorCodeTrade
            {
                Code = expectedTradeCode,
                Name = "trade"
            };
            await InMemoryDb.Instance.Trades.AddAsync(expectedTrade);
            return expectedTrade;
        }

        private static async Task<SORPriority> SeedPriority()
        {
            var expectedPriority = new SORPriority
            {
                Description = "priority",
                PriorityCode = 1
            };
            await InMemoryDb.Instance.SORPriorities.AddAsync(expectedPriority);
            return expectedPriority;
        }
    }
}
