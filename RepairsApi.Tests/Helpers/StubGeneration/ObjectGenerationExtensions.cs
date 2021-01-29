using Bogus;
using RepairsApi.V2.Generated;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RepairsApi.Tests.Helpers.StubGeneration
{
    public static class ObjectGenerationExtensions
    {
        public static Generator<T> AddDefaultGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddGenerator(new RandomStringGenerator(10))
                .AddGenerator(new RandomDoubleGenerator(0, 50))
                .AddGenerator(new RandomBoolGenerator());
        }

        public static Generator<T> AddValue<T, TValue>(this Generator<T> generator, TValue value)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TValue>(value));
        }

        public static Generator<T> AddGenerator<T, TValue>(this Generator<T> generator, Func<TValue> valueGenerator)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TValue>(valueGenerator));
        }

        public static Generator<T> AddValue<T, TModel, TProp>(this Generator<T> generator, TProp value, params Expression<Func<TModel, TProp>>[] accessors)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TProp>(value), accessors);
        }

        public static Generator<T> AddGenerator<T, TModel, TProp>(this Generator<T> generator, Func<TProp> valueGenerator, params Expression<Func<TModel, TProp>>[] accessors)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TProp>(valueGenerator), accessors);
        }

        public static Generator<T> AddWorkOrderGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddDefaultGenerators()
                .AddValue(new string[] { "address", "line" }, (PropertyAddress addr) => addr.AddressLine)
                .AddValue(new string[] { "address", "line" }, (Address addr) => addr.AddressLine)
                .AddValue(GetSitePropertyUnitGenerator(), (RaiseRepair rr) => rr.SitePropertyUnit)
                .AddValue(new double[] { 2.0 }, (Quantity q) => q.Amount)
                .AddValue(new string[] { "2.0" },
                    (GeographicalLocation q) => q.Latitude,
                    (GeographicalLocation q) => q.Longitude,
                    (GeographicalLocation q) => q.Elevation,
                    (GeographicalLocation q) => q.ElevationReferenceSystem)
                .AddValue("trade", (Trade t) => t.CustomCode);
        }

        public static Generator<T> WithSorCodes<T>(this Generator<T> generator, params string[] sorCodes)
        {
            Random rand = new Random();

            return generator.AddGenerator(() => sorCodes[rand.Next(sorCodes.Length)], (RateScheduleItem rsi) => rsi.CustomCode);
        }

        public static Generator<T> AddInfrastructureWorkOrderGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddDefaultGenerators()
                .AddValue(null, (RepairsApi.V2.Infrastructure.WorkOrder wo) => wo.WorkOrderComplete);
        }

        private static ICollection<SitePropertyUnit> GetSitePropertyUnitGenerator()
        {
            var generator = new Generator<SitePropertyUnit>()
                .AddValue(new string[] { "address", "line" }, (RepairsApi.V2.Generated.PropertyAddress addr) => addr.AddressLine)
                .AddValue(new string[] { "address", "line" }, (RepairsApi.V2.Generated.Address addr) => addr.AddressLine);
            return generator.GenerateList(1);
        }

        public static Generator<T> AddWorkOrderCompleteGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddWorkOrderGenerators()
                .AddValue(null, (RepairsApi.V2.Generated.WorkOrderComplete woc) => woc.FollowOnWorkOrderReference)
                .AddValue(null, (RepairsApi.V2.Generated.JobStatusUpdates jsu) => jsu.RelatedWorkElementReference)
                .AddValue(null, (RepairsApi.V2.Generated.JobStatusUpdates jsu) => jsu.AdditionalWork);
        }
    }
}
