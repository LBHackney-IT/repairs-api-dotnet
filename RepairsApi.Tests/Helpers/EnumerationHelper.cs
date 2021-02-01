using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.Helpers
{
    public static class EnumerationHelper
    {
        public static void AssertForEach<TActual, TExpected>(this IEnumerable<TActual> actualList, IEnumerable<TExpected> expectedList, Action<TActual, TExpected> asserter)
        {
            actualList.Should().HaveCount(expectedList.Count());

            var actualEnumerator = actualList.GetEnumerator();
            var expectedEnumerator = expectedList.GetEnumerator();

            while (actualEnumerator.MoveNext() && expectedEnumerator.MoveNext())
            {
                asserter(actualEnumerator.Current, expectedEnumerator.Current);
            };
        }
    }
}
