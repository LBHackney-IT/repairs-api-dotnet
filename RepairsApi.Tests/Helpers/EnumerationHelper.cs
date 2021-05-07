using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            }
            ;
        }

        public static string[] GetStaticValues(IReflect type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static |
                                  BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => (string) fi.GetValue(null))
                .ToArray();
        }

        public static string[] GetStaticValuesWithExclude(IReflect type, string exclude)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static |
                                  BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => (string) fi.GetValue(null))
                .Where(f => exclude != f)
                .ToArray();
        }
    }
}
