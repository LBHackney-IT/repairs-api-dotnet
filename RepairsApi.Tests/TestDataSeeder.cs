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
    public static class TestDataSeeder
    {
        private static object _lockObj = new object();

        private static int _intSeeder = 1;

        private static bool _seeded = false;

        private static Random _rand = new Random();

        private static List<string> _contractReferences = new List<string>();

        private static List<string> _contractorReferences = new List<string>();

        private static List<string> _tradeCodes = new List<string>();

        private static List<string> _sorCodes = new List<string>();

        private static List<string> _propertyReferences = new List<string>();

        private static List<int> _priorityCodes = new List<int>();

        public static void SeedData(this RepairsContext ctx)
        {
            lock (_lockObj)
            {
                if (!_seeded)
                {
                    SeedTrades(ctx);

                    SeedPriorities(ctx);

                    SeedSor(ctx);

                    SeedPropref();

                    SeedContractors(ctx);

                    SeedContracts(ctx);

                    SeedPropertyMap(ctx);

                    SeedSorMap(ctx);

                    _seeded = true;
                }
            }
        }

        private static void SeedSorMap(RepairsContext ctx)
        {
            var generator = new Generator<SORContract>()
                .AddDefaultGenerators()
                .AddGenerator(() => PickCode(_contractReferences), (SORContract c) => c.ContractReference)
                .AddGenerator(() => PickCode(_sorCodes), (SORContract c) => c.SorCodeCode)
                .Ignore((SORContract c) => c.Contract)
                .Ignore((SORContract c) => c.SorCode);

            IEnumerable<SORContract> entities = generator.GenerateList(100)
                .Distinct(new DelegatedComparator<SORContract>((sc1, sc2) => sc1.SorCodeCode == sc1.SorCodeCode && sc1.ContractReference == sc2.ContractReference));

            ctx.Set<SORContract>().AddRange(entities);
        }

        private static void SeedPropertyMap(RepairsContext ctx)
        {
            var generator = new Generator<PropertyContract>()
                .AddDefaultGenerators()
                .AddGenerator(() => PickCode(_contractReferences), (PropertyContract c) => c.ContractReference)
                .AddGenerator(() => PickCode(_propertyReferences), (PropertyContract c) => c.PropRef)
                .Ignore((PropertyContract c) => c.Contract);

            IEnumerable<PropertyContract> entities = generator.GenerateList(100)
                .Distinct(new DelegatedComparator<PropertyContract>((pc1, pc2) => pc1.PropRef == pc2.PropRef && pc1.ContractReference == pc2.ContractReference));

            ctx.Set<PropertyContract>().AddRange(entities);
        }

        private static void SeedContracts(RepairsContext ctx)
        {
            var generator = new Generator<Contract>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(_contractReferences), (Contract c) => c.ContractReference)
                // TODO Seed Dates
                .AddGenerator(() => PickCode(_contractorReferences), (Contract c) => c.ContractorReference)
                .Ignore((Contract c) => c.PropertyMap)
                .Ignore((Contract c) => c.SorCodeMap)
                .Ignore((Contract c) => c.Contractor);

            ctx.Set<Contract>().AddRange(generator.GenerateList(100));
        }

        private static void SeedContractors(RepairsContext ctx)
        {
            var generator = new Generator<Contractor>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(_contractorReferences), (Contractor c) => c.Reference);

            ctx.Set<Contractor>().AddRange(generator.GenerateList(100));
        }

        private static void SeedPropref()
        {
            for (int i = 0; i < 100; i++)
            {
                SeedCode(_propertyReferences);
            }
        }

        private static void SeedSor(RepairsContext ctx)
        {
            var generator = new Generator<ScheduleOfRates>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(_sorCodes), (ScheduleOfRates sor) => sor.CustomCode)
                .AddGenerator(() => PickCode(_tradeCodes), (ScheduleOfRates sor) => sor.TradeCode)
                .AddGenerator(() => PickCode(_priorityCodes), (ScheduleOfRates sor) => sor.PriorityId)
                .Ignore((ScheduleOfRates sor) => sor.Priority)
                .Ignore((ScheduleOfRates sor) => sor.Trade)
                .Ignore((ScheduleOfRates sor) => sor.SorCodeMap);

            ctx.Set<ScheduleOfRates>().AddRange(generator.GenerateList(100));
        }

        private static void SeedPriorities(RepairsContext ctx)
        {
            var generator = new Generator<SORPriority>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(_priorityCodes), (SORPriority sp) => sp.PriorityCode);

            ctx.Set<SORPriority>().AddRange(generator.GenerateList(100));
        }

        private static void SeedTrades(RepairsContext ctx)
        {
            var generator = new Generator<SorCodeTrade>()
                .AddDefaultGenerators()
                .AddGenerator(() => SeedCode(_tradeCodes), (SorCodeTrade sct) => sct.Code);

            ctx.Set<SorCodeTrade>().AddRange(generator.GenerateList(100));
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
