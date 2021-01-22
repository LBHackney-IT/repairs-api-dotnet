using System.Collections.Generic;

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

        public static Generator<T> AddWorkOrderGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddDefaultGenerators()
                .AddGenerator(new SimpleValueGenerator<ICollection<string>>(new string[] { "hello", "me" }), (RepairsApi.V2.Generated.PropertyAddress addr) => addr.AddressLine)
                .AddGenerator(new SimpleValueGenerator<ICollection<double>>(new double[] { 2.0 }), (RepairsApi.V2.Generated.Quantity q) => q.Amount)
                .AddGenerator(new SimpleValueGenerator<ICollection<string>>(new string[] { "2.0" }),
                    (RepairsApi.V2.Generated.GeographicalLocation q) => q.Latitude,
                    (RepairsApi.V2.Generated.GeographicalLocation q) => q.Longitude,
                    (RepairsApi.V2.Generated.GeographicalLocation q) => q.Elevation,
                    (RepairsApi.V2.Generated.GeographicalLocation q) => q.ElevationReferenceSystem);
        }
    }
}
