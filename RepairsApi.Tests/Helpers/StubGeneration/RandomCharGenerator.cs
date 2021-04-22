using Bogus;
using System;

namespace RepairsApi.Tests.Helpers.StubGeneration
{
    internal class RandomCharGenerator : AbstractValueGenerator<char>
    {
        private readonly RandomStringGenerator _internalGenerator;

        public RandomCharGenerator()
        {
            _internalGenerator = new RandomStringGenerator(1);
        }

        public override char GenerateValue()
        {
            return _internalGenerator.GenerateValue()[0];
        }
    }
}
