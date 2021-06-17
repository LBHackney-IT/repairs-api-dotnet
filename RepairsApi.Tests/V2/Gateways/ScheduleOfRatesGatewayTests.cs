using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
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
            const string expectedProperty = "property";
            var expectedPriority = await SeedPriority();
            string contractorReference = "contractor";
            await SeedContractor(contractorReference);
            var expectedTrade = await SeedTrade(Guid.NewGuid().ToString());
            var expectedTrade1 = await SeedTrade(Guid.NewGuid().ToString(), "second Trade");
            var validContracts = await SeedContracts(expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7), contractorReference);
            await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade, validContracts.First());
            await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade1, validContracts.First());
            await InMemoryDb.Instance.SaveChangesAsync();
            // act
            var result = await _classUnderTest.GetTrades(expectedProperty);

            // assert
            result.Single().Should().BeEquivalentTo(expectedTrade);
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
                ContractReference = "contract",
                TerminationDate = DateTime.UtcNow.Date.AddYears(1),
                EffectiveDate = DateTime.UtcNow.Date.AddDays(-7)
            };

            ScheduleOfRates sor = new ScheduleOfRates
            {
                Code = "code",
                Cost = 1,
                Enabled = true,
                LongDescription = "",
            };

            generator
                .AddDefaultGenerators()
                .AddValue(sor, (SORContract c) => c.SorCode)
                .AddValue(contract, (SORContract c) => c.Contract);
            var expectedContract = generator.Generate();

            await InMemoryDb.Instance.SORContracts.AddAsync(expectedContract);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var result = await _classUnderTest.GetCost(contract.ContractorReference, sor.Code);

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

        [Test]
        public async Task OnlyReturnsEnabled()
        {
            // arrange
            const string expectedProperty = "property";
            var expectedPriority = await SeedPriority();
            string contractorReference = "contractor";
            await SeedContractor(contractorReference);
            var expectedTrade = await SeedTrade(Guid.NewGuid().ToString());
            var validContracts = await SeedContracts(expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7), contractorReference);

            await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade, validContracts.First(), false);
            var expectedCodes = await SeedSorCodes(expectedPriority, expectedProperty, expectedTrade, validContracts.First());

            await InMemoryDb.Instance.SaveChangesAsync();

            await GetAndValidateCodes(expectedProperty, expectedTrade, contractorReference, expectedCodes);
        }

        [Test]
        public async Task GetContractManagerEmail()
        {
            const string ExpectedEmail = "email@email.email";
            const string Reference = "reference";
            InMemoryDb.Instance.Contractors.Add(new Contractor
            {
                Reference = Reference,
                Name = "contractor",
                ContractManagerEmail = ExpectedEmail
            });
            await InMemoryDb.Instance.SaveChangesAsync();
            var email = await _classUnderTest.GetContractManagerEmail(Reference);

            email.Should().Be(ExpectedEmail);
        }

        [Test]
        public async Task GetContractManagerEmailIsEmptyWhenNotSet()
        {
            const string Reference = "reference";
            InMemoryDb.Instance.Contractors.Add(new Contractor
            {
                Reference = Reference,
                Name = "contractor"
            });
            await InMemoryDb.Instance.SaveChangesAsync();
            var email = await _classUnderTest.GetContractManagerEmail(Reference);

            email.Should().BeNull();
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

        [Test]
        public async Task ListsContractorsFilteredByPropRefAndTrade()
        {
            const string expectedProperty = "property";
            var expectedTrade = await SeedTrade("trade");
            await SeedContractors(expectedTrade.Code, "not" + expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7));
            await SeedContractors("not" + expectedTrade.Code, expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7));
            await SeedContractors(expectedTrade.Code, expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(-1));
            await SeedContractors(expectedTrade.Code, expectedProperty, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(7));
            var expectedContractors = await SeedContractors(expectedTrade.Code, expectedProperty, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7));

            var result = await _classUnderTest.GetContractors(expectedProperty, expectedTrade.Code);

            result.Should().HaveCount(expectedContractors.Count);
        }

        [Test]
        public async Task CanGetContractorByRef()
        {
            const string expectedContractorRef = "contractorRef";
            var contractorGenerator = new Generator<Contractor>()
                .AddDefaultGenerators()
                .AddValue(null, (Contractor c) => c.Contracts);
            var contractors = contractorGenerator.GenerateList(10);
            var expectedContractor = contractorGenerator.Generate();
            expectedContractor.Reference = expectedContractorRef;
            await InMemoryDb.Instance.Contractors.AddRangeAsync(contractors.Append(expectedContractor));
            await InMemoryDb.Instance.SaveChangesAsync();

            var result = await _classUnderTest.GetContractor(expectedContractorRef);

            result.ContractorReference.Should().Be(expectedContractor.Reference);
            result.ContractorName.Should().Be(expectedContractor.Name);
            result.UseExternalScheduleManager.Should().Be(expectedContractor.UseExternalScheduleManager);
        }

        [Test]
        public async Task GetContractorReturnsUseExternalScheduleManager()
        {
            var contractorGenerator = new Generator<Contractor>()
                .AddDefaultGenerators()
                .AddValue(null, (Contractor c) => c.Contracts);
            var externalContractor = contractorGenerator.Generate();
            externalContractor.UseExternalScheduleManager = true;
            var notExternalContractor = contractorGenerator.Generate();
            notExternalContractor.UseExternalScheduleManager = false;
            await InMemoryDb.Instance.Contractors.AddAsync(externalContractor);
            await InMemoryDb.Instance.Contractors.AddAsync(notExternalContractor);
            await InMemoryDb.Instance.SaveChangesAsync();

            var externalResult = await _classUnderTest.GetContractor(externalContractor.Reference);
            var notExternalResult = await _classUnderTest.GetContractor(notExternalContractor.Reference);

            externalResult.UseExternalScheduleManager.Should().Be(externalContractor.UseExternalScheduleManager);
            notExternalResult.UseExternalScheduleManager.Should().Be(notExternalContractor.UseExternalScheduleManager);
        }

        private async Task GetAndValidateCodes(string expectedProperty, SorCodeTrade expectedTrade, string contractorReference, List<ScheduleOfRates> expectedCodes)
        {

            // act
            var result = await _classUnderTest.GetSorCodes(expectedProperty, expectedTrade.Code, contractorReference);

            // assert
            var expectedResult = expectedCodes.Select(sor => new ScheduleOfRatesModel
            {
                Code = sor.Code,
                ShortDescription = sor.ShortDescription,
                LongDescription = sor.LongDescription,
                Priority = new RepairsApi.V2.Domain.SORPriority
                {
                    PriorityCode = sor.Priority?.PriorityCode,
                    Description = sor.Priority?.Description,
                },
                Cost = sor.Cost
            }).ToList();
            result.Should().BeEquivalentTo(expectedResult);
        }

        private static async Task<List<ScheduleOfRates>> SeedSorCodes(
            SORPriority expectedPriority,
            string expectedProperty,
            SorCodeTrade expectedTrade,
            Contract expectedContract = null,
            bool enabled = true)
        {
            var expectedGenerator = new Generator<ScheduleOfRates>();

            expectedGenerator
                .AddDefaultGenerators()
                .AddValue(expectedPriority, (ScheduleOfRates sor) => sor.Priority)
                .AddValue(expectedProperty, (PropertyContract pc) => pc.PropRef)
                .AddValue(expectedTrade, (ScheduleOfRates sor) => sor.Trade)
                .AddValue(enabled, (ScheduleOfRates sor) => sor.Enabled)
                .AddGenerator(() => GenerateJoinEntry(expectedContract), (ScheduleOfRates sor) => sor.SorCodeMap);
            var expectedCodes = expectedGenerator.GenerateList(10);

            await InMemoryDb.Instance.SORCodes.AddRangeAsync(expectedCodes);
            return expectedCodes;
        }

        private static List<SORContract> GenerateJoinEntry(Contract expectedContract)
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

        private static async Task<List<Contractor>> SeedContractors(string tradeCode, string expectedProperty, DateTime effectiveDate, DateTime termDate)
        {
            var stringGenerator = new Generator<string>()
                .AddDefaultGenerators();
            var contractorGenerator = new Generator<Contractor>()
                .AddDefaultGenerators()
                .AddValue(null, (Contractor c) => c.Contracts);

            var contractors = contractorGenerator.GenerateList(10);
            var contracts = contractors.Select(c => new Contract
            {
                ContractReference = stringGenerator.Generate(),
                ContractorReference = c.Reference,
                EffectiveDate = effectiveDate,
                TerminationDate = termDate
            }).ToList();
            var sorCode = new ScheduleOfRates
            {
                Code = stringGenerator.Generate(),
                TradeCode = tradeCode,
                Enabled = true
            };
            var sorContracts = contracts.Select(c => new SORContract
            {
                ContractReference = c.ContractReference,
                SorCodeCode = sorCode.Code,
                Cost = null
            }).ToList();

            var propMaps = contracts.Select(c => new PropertyContract
            {
                PropRef = expectedProperty,
                ContractReference = c.ContractReference
            }).ToList();

            await InMemoryDb.Instance.Contractors.AddRangeAsync(contractors);
            await InMemoryDb.Instance.SORCodes.AddAsync(sorCode);
            await InMemoryDb.Instance.Contracts.AddRangeAsync(contracts);
            await InMemoryDb.Instance.SORContracts.AddRangeAsync(sorContracts);
            await InMemoryDb.Instance.PropertyContracts.AddRangeAsync(propMaps);
            await InMemoryDb.Instance.SaveChangesAsync();

            return contractors;
        }

        private static async Task<SorCodeTrade> SeedTrade(string expectedTradeCode, string name="trade")
        {
            var expectedTrade = new SorCodeTrade
            {
                Code = expectedTradeCode,
                Name = name
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
