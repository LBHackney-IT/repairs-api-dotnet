using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Generated;

namespace RepairsApi.Tests.V1.Factories
{
    public class DBModelFactoryTests
    {
        [Test]
        public void ThrowsExceptionWhenMultipleAmountsProvided()
        {
            var quatity = new Quantity
            {
                Amount = new List<double>
                {
                    1.2, 5.6, 14.58
                }
            };

            Assert.Throws<NotSupportedException>(() => quatity.ToDb())
                .Message.Should().Be("Multiple amounts is not supported");
        }
    }
}
