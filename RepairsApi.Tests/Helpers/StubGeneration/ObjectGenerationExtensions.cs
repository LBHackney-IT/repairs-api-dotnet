namespace RepairsApi.Tests.Helpers.StubGeneration
{
    public static class ObjectGenerationExtensions
    {
        public static Generator<T> AddDefaultValueGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddGenerator(new RandomStringGenerator(10))
                .AddGenerator(new RandomDoubleGenerator(0, 50))
                .AddGenerator(new RandomBoolGenerator());
        }
    }
}
