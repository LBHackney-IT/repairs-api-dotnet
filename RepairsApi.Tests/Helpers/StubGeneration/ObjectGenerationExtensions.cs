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

        public static Generator<T> AddGenerator<T, TValue>(this Generator<T> generator, TValue value)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TValue>(value));
        }

        public static Generator<T> AddGenerator<T, TValue>(this Generator<T> generator, Func<TValue> valueGenerator)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TValue>(valueGenerator));
        }

        public static Generator<T> AddGenerator<T, TModel, TProp>(this Generator<T> generator, TProp value, params Expression<Func<TModel, TProp>>[] accessors)
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
                .AddGenerator(new string[] { "address", "line" }, (RepairsApi.V2.Generated.PropertyAddress addr) => addr.AddressLine)
                .AddGenerator(new double[] { 2.0 }, (RepairsApi.V2.Generated.Quantity q) => q.Amount)
                .AddGenerator(new string[] { "2.0" },
                    (RepairsApi.V2.Generated.GeographicalLocation q) => q.Latitude,
                    (RepairsApi.V2.Generated.GeographicalLocation q) => q.Longitude,
                    (RepairsApi.V2.Generated.GeographicalLocation q) => q.Elevation,
                    (RepairsApi.V2.Generated.GeographicalLocation q) => q.ElevationReferenceSystem);
        }
    }
}
