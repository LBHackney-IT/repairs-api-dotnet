using System;

namespace RepairsApi.Tests.Helpers.StubGeneration
{
    public class RandomEnumGenerator<T> : AbstractValueGenerator<T> where T : Enum
    {
        private static readonly Random _random = new Random();

        public override T GenerateValue()
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }
    }
}
