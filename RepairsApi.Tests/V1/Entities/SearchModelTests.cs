using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V1.UseCase;

namespace RepairsApi.Tests.V1.Entities
{
    public class SearchModelTests
    {
        [Test]
        public void PostCodePrioritised()
        {
            PropertySearchModel searchModel = new PropertySearchModel
            {
                PostCode = "PostCode",
                Address = "Address",
                Query = "Query"
            };

            var queryParam = searchModel.GetQueryParameter();

            queryParam.Should().Be("postcode=PostCode");
        }

        [Test]
        public void AddressPrioritisedOverQuery()
        {
            PropertySearchModel searchModel = new PropertySearchModel
            {
                Address = "Address",
                Query = "Query"
            };

            var queryParam = searchModel.GetQueryParameter();

            queryParam.Should().Be("address=Address");
        }

        [TestCase("OL16 4 PP")]
        [TestCase("HU7 4XF")]
        [TestCase("s 14 1 JH")]
        [TestCase("NR17   1bX")]
        [TestCase("SE21  7BP")]
        [TestCase("N1 3 4Bb")]
        [TestCase("PA62 6AA")]
        public void PostCodeIsUsedIfIsValidPostCode(string query)
        {
            PropertySearchModel searchModel = new PropertySearchModel
            {
                Query = query
            };

            var queryParam = searchModel.GetQueryParameter();

            queryParam.Should().Be($"postcode={query}");
        }


        [TestCase("test")]
        [TestCase("1 road drive")]
        [TestCase("london")]
        [TestCase("ng7 7ppp")]
        [TestCase("a")]
        [TestCase("N1 333 4Bb")]
        [TestCase("Lorem ipsum dolor sit amet, consectetur cras amet.")]
        public void AddressIsUsedIfIsNotValidPostCode(string query)
        {
            PropertySearchModel searchModel = new PropertySearchModel
            {
                Query = query
            };

            var queryParam = searchModel.GetQueryParameter();

            queryParam.Should().Be($"address={query}");
        }

        [Test]
        public void InvalidWhenNotDefined()
        {
            PropertySearchModel searchModel = new PropertySearchModel
            {
            };

            searchModel.IsValid().Should().BeFalse();
        }

        [Test]
        public void InvalidWhenEmptyOrWhiteSpace()
        {
            PropertySearchModel searchModel = new PropertySearchModel
            {
                Address = "",
                PostCode = " ",
                Query = "  "
            };

            searchModel.IsValid().Should().BeFalse();
        }
    }
}
