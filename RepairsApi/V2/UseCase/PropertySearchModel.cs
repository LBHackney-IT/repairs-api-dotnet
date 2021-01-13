using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class PropertySearchModel
    {
        static Regex _postCodeMatcher = new Regex("^[A-Z]{1,2}([0-9]{1,2}|[0-9][A-Z])[0-9][A-Z]{2}$");

        public string Address { get; set; }
        public string PostCode { get; set; }
        public string Query { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Address) || !string.IsNullOrWhiteSpace(PostCode) || !string.IsNullOrWhiteSpace(Query);
        }

        public string GetQueryParameter()
        {
            if (!string.IsNullOrWhiteSpace(PostCode)) return PostCodeQuery(PostCode);
            if (!string.IsNullOrWhiteSpace(Address)) return AddressQuery(Address);
            if (!string.IsNullOrWhiteSpace(Query)) return IsPostCode(Query) ? PostCodeQuery(Query) : AddressQuery(Query);

            return string.Empty;
        }

        private static bool IsPostCode(string query)
        {
            var normalisedQuery = query.Replace(" ", "").ToUpper();

            return _postCodeMatcher.IsMatch(normalisedQuery);
        }

        private static string AddressQuery(string value)
        {
            return $"address={value}";
        }

        private static string PostCodeQuery(string value)
        {
            return $"postcode={value}";
        }

    }
}
