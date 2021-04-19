using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Filtering
{
    public class FilterTests
    {
        [Test]
        public void AppliesFilter()
        {
            FilterBuilder<TestSearch, TestItem> builder = BuilderFilter();

            var filter = builder.BuildFilter(new TestSearch(5));

            List<TestItem> items = BuildTestItems();

            var result = filter.Apply(items);

            result.Should().HaveCount(4);
        }

        [Test]
        public void DoesntApplyFilterIfNotValid()
        {
            FilterBuilder<TestSearch, TestItem> builder = BuilderFilter();

            var filter = builder.BuildFilter(new TestSearch(0));

            List<TestItem> items = BuildTestItems();

            var result = filter.Apply(items);

            result.Should().HaveCount(items.Count);
        }

        private static List<TestItem> BuildTestItems()
        {
            return new List<TestItem>()
            {
                new TestItem(1),
                new TestItem(2),
                new TestItem(3),
                new TestItem(4),
                new TestItem(5),
                new TestItem(6),
                new TestItem(7),
                new TestItem(8),
                new TestItem(9),
            };
        }

        private static FilterBuilder<TestSearch, TestItem> BuilderFilter()
        {
            return new FilterBuilder<TestSearch, TestItem>()
                .AddFilter(
                    search => search.SearchParam,
                    searchParam => searchParam > 0,
                    searchParam => item => item.Value > searchParam);
        }
    }

    class TestSearch
    {
        public TestSearch(int searchParam)
        {
            SearchParam = searchParam;
        }

        public int SearchParam { get; set; }
    }

    class TestItem
    {
        public TestItem(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}
