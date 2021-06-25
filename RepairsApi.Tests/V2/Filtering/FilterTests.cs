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

            var items = BuildTestItems().AsQueryable();

            var result = filter.Apply(items);

            result.Should().HaveCount(4);
        }

        [Test]
        public void DoesntApplyFilterIfNotValid()
        {
            FilterBuilder<TestSearch, TestItem> builder = BuilderFilter();

            var filter = builder.BuildFilter(new TestSearch(0));

            var items = BuildTestItems().AsQueryable();

            var result = filter.Apply(items);

            result.Should().HaveCount(items.Count());
        }

        [Test]
        public void AppliesAscendingSort()
        {
            FilterBuilder<TestSearch, TestItem> builder = BuilderFilter();

            var ascendingFilter = builder.BuildFilter(new TestSearch("value:asc"));

            var items = BuildTestItems().AsQueryable();

            var ascendingResult = ascendingFilter.Apply(items);

            ascendingResult.Should().BeInAscendingOrder(ti => ti.Value);
        }

        [Test]
        public void AppliesDescendingSort()
        {
            FilterBuilder<TestSearch, TestItem> builder = BuilderFilter();

            var descendingFilter = builder.BuildFilter(new TestSearch("value:desc"));

            var items = BuildTestItems().AsQueryable();

            var descendingResult = descendingFilter.Apply(items);

            descendingResult.Should().BeInDescendingOrder(ti => ti.Value);
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
                    searchParam => item => item.Value > searchParam)
                .AddSort(search => search.Sort, b => b.AddSortOption("value", ti => ti.Value)); ;
        }
    }

    class TestSearch
    {
        public string Sort;
        public int SearchParam { get; set; }

        public TestSearch(int searchParam)
        {
            SearchParam = searchParam;
        }

        public TestSearch(string sortParam)
        {
            Sort = sortParam;
        }
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
