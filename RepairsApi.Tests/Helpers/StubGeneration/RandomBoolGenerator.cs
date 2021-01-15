using System;

namespace RepairsApi.Tests.Helpers.StubGeneration
{
    public class RandomBoolGenerator : AbstractValueGenerator<bool>
    {
        private static readonly Random _random = new Random();

        public override bool GenerateValue()
        {
            return _random.NextDouble() >= 0.5;
        }
    }
}
