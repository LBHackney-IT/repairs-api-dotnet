using Bogus;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepairsApi.Tests
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Ignore duplicate adds")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "Access Seeded Data")]
    public static class TestDataSeeder
    {
        public const string ContractReference = "contract";
        public const string SorCode = "code";
        public const string PropRef = "propref";
        public const string Contractor = "contractor";
        public const string Trade = "trade";
        public const string Priority = "priority";
        private const int PriorityId = 1;
        private static object _lockObj = new object();

        private static int _intSeeder = 2;

        private static Random _rand = new Random();
        public static List<string> ContractReferences = new List<string>();

        public static List<string> ContractorReferences = new List<string>();

        public static List<string> TradeCodes = new List<string>();

        public static List<string> SorCodes = new List<string>();

        public static List<string> PropertyReferences = new List<string>();

        public static List<int> PriorityCodes = new List<int>();

        public static void SeedData(this RepairsContext ctx)
        {

            lock (_lockObj)
            {
                var hasSeedData = ctx.Trades.Any();

                if (!hasSeedData)
                {
                    SeedTrades(ctx);

                    SeedPriorities(ctx);

                    SeedSor(ctx);

                    SeedPropref();

                    SeedContractors(ctx);

                    SeedContracts(ctx);

                    SeedPropertyMap(ctx);

                    SeedSorMap(ctx);
                }
            }
        }

        private static void SeedSorMap(RepairsContext ctx)
        {
            var generator = new Generator<SORContract>()
                .AddDefaultGenerators()
                .AddGenerator(() => PickCode(ContractReferences), (SORContract c) => c.ContractReference)
                .AddGenerator(() => PickCode(SorCodes), (SORContract c) => c.SorCodeCode)
                .Ignore((SORContract c) => c.Contract)
                .Ignore((SORContract c) => c.SorCode);

            IEnumerable<SORContract> entities = generator.GenerateList(100).Distinct();

            ctx.Set<SORContract>().AddRange(entities);
            ctx.Set<SORContract>().Add(new SORContract
            {
                ContractReference = ContractReference,
                SorCodeCode = SorCode,
                Cost = 1
            });
        }

        private static void SeedPropertyMap(RepairsContext ctx)
        {
            var generator = new Generator<PropertyContract>()
                .AddDefaultGenerators()
                .AddGenerator(() => PickCode(ContractReferences), (PropertyContract c) => c.ContractReference)
                .AddGenerator(() => PickCode(PropertyReferences), (PropertyContract c) => c.PropRef)
                .Ignore((PropertyContract c) => c.Contract);

            IEnumerable<PropertyContract> entities = generator.GenerateList(100).Distinct();

            ctx.Set<PropertyContract>().AddRange(entities);
            ctx.Set<PropertyContract>().Add(new PropertyContract
            {
                ContractReference = ContractReference,
                PropRef = PropRef
            });
        }

        private static void SeedContracts(RepairsContext ctx)
        {
            var generator = new Generator<Contract>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(ContractReferences), (Contract c) => c.ContractReference)
                .AddValue(DateTime.UtcNow.AddDays(-1), (Contract c) => c.EffectiveDate)
                .AddValue(DateTime.UtcNow.AddDays(1), (Contract c) => c.TerminationDate)
                .AddGenerator(() => PickCode(ContractorReferences), (Contract c) => c.ContractorReference)
                .Ignore((Contract c) => c.PropertyMap)
                .Ignore((Contract c) => c.SorCodeMap)
                .Ignore((Contract c) => c.Contractor);

            ctx.Set<Contract>().AddRange(generator.GenerateList(100));
            ctx.Set<Contract>().Add(new Contract
            {
                ContractorReference = Contractor,
                ContractReference = ContractReference,
                EffectiveDate = DateTime.UtcNow.AddDays(-1),
                TerminationDate = DateTime.UtcNow.AddDays(1),
            });
        }

        private static void SeedContractors(RepairsContext ctx)
        {
            var generator = new Generator<Contractor>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(ContractorReferences), (Contractor c) => c.Reference)
                .AddValue(null, (Contractor c) => c.Contracts);

            ctx.Set<Contractor>().AddRange(generator.GenerateList(100));
            ctx.Set<Contractor>().Add(new Contractor
            {
                Name = Contractor,
                Reference = Contractor
            });
        }

        private static void SeedPropref()
        {
            for (int i = 0; i < 100; i++)
            {
                SeedCode(PropertyReferences);
            }
        }

        private static void SeedSor(RepairsContext ctx)
        {
            var generator = new Generator<ScheduleOfRates>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(SorCodes), (ScheduleOfRates sor) => sor.CustomCode)
                .AddGenerator(() => PickCode(TradeCodes), (ScheduleOfRates sor) => sor.TradeCode)
                .AddGenerator(() => PickCode(PriorityCodes), (ScheduleOfRates sor) => sor.PriorityId)
                .Ignore((ScheduleOfRates sor) => sor.Priority)
                .Ignore((ScheduleOfRates sor) => sor.Trade)
                .Ignore((ScheduleOfRates sor) => sor.SorCodeMap);

            ctx.Set<ScheduleOfRates>().AddRange(generator.GenerateList(100));
            ctx.Set<ScheduleOfRates>().Add(new ScheduleOfRates
            {
                Cost = 1,
                CustomCode = SorCode,
                CustomName = SorCode,
                PriorityId = PriorityId,
                TradeCode = Trade
            });
        }

        private static void SeedPriorities(RepairsContext ctx)
        {
            var generator = new Generator<SORPriority>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(PriorityCodes), (SORPriority sp) => sp.PriorityCode);

            ctx.Set<SORPriority>().AddRange(generator.GenerateList(100));
            ctx.Set<SORPriority>().Add(new SORPriority
            {
                Description = Priority,
                PriorityCode = PriorityId
            });
        }

        private static void SeedTrades(RepairsContext ctx)
        {
            var generator = new Generator<SorCodeTrade>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(TradeCodes), (SorCodeTrade sct) => sct.Code);

            ctx.Set<SorCodeTrade>().AddRange(generator.GenerateList(100));

            ctx.Set<SorCodeTrade>().Add(new SorCodeTrade
            {
                Code = Trade,
                Name = Trade
            });
        }

        public static T SeedCode<T>(List<T> values)
        {
            T newValue;

            do
            {
                newValue = new Generator<T>()
                    .AddDefaultGenerators()
                    .AddGenerator(() => _intSeeder++)
                    .Generate();
            } while (values.Contains(newValue));

            values.Add(newValue);

            return newValue;
        }

        public static T PickCode<T>(List<T> values)
        {
            if (values.Count <= 0) return default(T);

            return values[_rand.Next(values.Count)];
        }
    }
}
