using System;
using System.Linq;

namespace RepairsApi.Tests.Helpers.StubGeneration
{
    public class RandomStringGenerator : AbstractValueGenerator<string>
    {
        private readonly int _length;
        private static readonly Random _random = new Random();

        public RandomStringGenerator(int length)
        {
            _length = length;
        }

        public override string GenerateValue()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, _length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
