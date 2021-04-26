using System;

namespace RepairsApi.Tests.Helpers.StubGeneration
{
    public class RandomDateGenerator : AbstractValueGenerator<DateTime>
    {
        private static readonly Random _random = new Random();
        private readonly int _range;

        public RandomDateGenerator(int range)
        {
            _range = range;
        }

        public override DateTime GenerateValue()
        {
            return DateTime.UtcNow.AddDays(_random.Next(-_range/2, _range/2));
        }
    }
}
