using System;

namespace RepairsApi.Tests.Helpers.StubGeneration
{
    public class RandomDoubleGenerator : AbstractValueGenerator<double>
    {
        private readonly double _min;
        private readonly double _max;
        private static readonly Random _random = new Random();

        public RandomDoubleGenerator(double min = 0, double max = 10)
        {
            _min = min;
            _max = max;
        }

        public override double GenerateValue()
        {
            return (_random.NextDouble() * (_max - _min)) + _min;
        }
    }
}
