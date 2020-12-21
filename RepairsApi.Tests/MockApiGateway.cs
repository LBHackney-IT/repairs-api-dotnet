using RepairsApi.V1.Gateways;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static RepairsApi.Tests.V1.DataFakers;

namespace RepairsApi.Tests
{
    internal delegate RouteParams RouteMatcher(string url);
    internal delegate ApiResponse<object> RequestHandler(string value);

    public class MockApiGateway : IApiGateway
    {
        private Dictionary<RouteMatcher, RequestHandler> _router = new Dictionary<RouteMatcher, RequestHandler>(){
            { MatchAlerts, HandleAlerts },
            { MatchPostCodeSearch, HandlePostCodeSearch },
            { MatchAddressSearch, HandleAddressSearch },
            { MatchPropertyReference, HandlePropertyReference }
        };

        private static List<AlertsApiResponse> _alertsApiResponse;
        private static List<PropertyApiResponse> _propertyApiResponse;

        public static List<AlertsApiResponse> MockAlertsApiResponses
        {
            get
            {
                if (_alertsApiResponse is null)
                {
                    _alertsApiResponse = new List<AlertsApiResponse>(StubAlertApiResponse().Generate(20));
                };

                return _alertsApiResponse;
            }
        }

        public static List<PropertyApiResponse> MockPropertyApiResponses
        {
            get
            {
                if (_propertyApiResponse is null)
                {
                    _propertyApiResponse = new List<PropertyApiResponse>(StubPropertyApiResponse().Generate(40));
                };

                return _propertyApiResponse;
            }
        }

        public Task<ApiResponse<TResponse>> ExecuteRequest<TResponse>(Uri url) where TResponse : class
        {
            string urlString = url.ToString();

            foreach (var kv in _router)
            {
                RouteParams routeParams = kv.Key.Invoke(urlString);
                if (routeParams.Matches)
                {
                    ApiResponse<object> apiResponse = kv.Value.Invoke(routeParams.QueryValue);
                    return Task.FromResult(new ApiResponse<TResponse>(apiResponse.IsSuccess, apiResponse.Status, apiResponse.Content as TResponse));
                }
            }

            return null;
        }

        private static RouteParams MatchPropertyReference(string url)
        {
            return new RouteParams
            {
                Matches = url.Contains("testpropertiesapi"),
                QueryValue = url.Split("/").Last()
            };
        }

        private static RouteParams MatchPostCodeSearch(string url)
        {
            return new RouteParams
            {
                Matches = url.Contains("testpropertiesapi") && url.Contains("?postcode"),
                QueryValue = url.Split("=").Last()
            };
        }

        private static RouteParams MatchAddressSearch(string url)
        {
            return new RouteParams
            {
                Matches = url.Contains("testpropertiesapi") && url.Contains("?address"),
                QueryValue = url.Split("=").Last()
            };
        }

        private static RouteParams MatchAlerts(string url)
        {
            return new RouteParams
            {
                Matches = url.Contains("testalertsapi"),
                QueryValue = url.Split("/").Last()
            };
        }

        private static ApiResponse<object> HandlePropertyReference(string value)
        {
            return BuildResponse<object>(MockPropertyApiResponses.Where(res => res.PropRef == value).FirstOrDefault());
        }

        private static ApiResponse<object> HandlePostCodeSearch(string value)
        {
            return BuildResponse<object>(MockPropertyApiResponses.Where(prop => prop.PostCode.Equals(value)).ToList());
        }

        private static ApiResponse<object> HandleAddressSearch(string value)
        {
            return BuildResponse<object>(MockPropertyApiResponses.Where(prop => prop.Address1.Contains(value)).ToList());
        }

        private static ApiResponse<object> HandleAlerts(string value)
        {
            return BuildResponse<object>(MockAlertsApiResponses.Where(res => res.PropertyReference == value).FirstOrDefault());
        }

        internal static void AddProperties(int count, string postcode = null, string address = null)
        {
            var newProperties = StubPropertyApiResponse().Generate(count);

            if (postcode != null)
            {
                newProperties.ForEach(property => property.PostCode = postcode);
            }

            if (address != null)
            {
                newProperties.ForEach(property => property.Address1 = address);
            }

            MockPropertyApiResponses.AddRange(newProperties);
        }

        private static ApiResponse<T> BuildResponse<T>(T content) where T : class
        {
            if (content == null)
            {
                return new ApiResponse<T>(false, HttpStatusCode.NotFound, null);
            }

            return new ApiResponse<T>(true, HttpStatusCode.OK, content);
        }
    }

    internal class RouteParams
    {
        public bool Matches { get; set; }

        public string QueryValue { get; set; }
    }
}
