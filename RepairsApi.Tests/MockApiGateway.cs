using RepairsApi.V1.Factories;
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
        #region Data

        private static List<PropertyAlertsApiResponse> _propertyAlertsApiResponse;
        public static List<PropertyAlertsApiResponse> MockPropertyAlertsApiResponses
        {
            get
            {
                if (_propertyAlertsApiResponse is null)
                {
                    _propertyAlertsApiResponse = new List<PropertyAlertsApiResponse>(StubPropertyAlertApiResponse().Generate(20));
                };

                return _propertyAlertsApiResponse;
            }
        }

        private static List<PropertyApiResponse> _propertyApiResponse;
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

        private static List<PersonAlertsApiResponse> _personAlertsApiResponse;
        public static List<PersonAlertsApiResponse> MockPersonAlertsApiResponses
        {
            get
            {
                if (_personAlertsApiResponse is null)
                {
                    _personAlertsApiResponse = new List<PersonAlertsApiResponse>(StubPersonAlertApiResponse().Generate(20));
                };

                return _personAlertsApiResponse;
            }
        }


        private static List<TenancyApiTenancyInformation> _tenantApiResponse;
        public static List<TenancyApiTenancyInformation> MockTenantApiResponses
        {
            get
            {
                if (_tenantApiResponse is null)
                {
                    _tenantApiResponse = new List<TenancyApiTenancyInformation>(StubTenantApiResponse().Generate(40));
                };

                return _tenantApiResponse;
            }
            
        }
        #endregion

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

        #region Routing
        private Dictionary<RouteMatcher, RequestHandler> _router = new Dictionary<RouteMatcher, RequestHandler>(){
            { MatchTenancyInformation, HandleTenancyInformation },
            { MatchPersonAlerts, HandlePersonAlerts },
            { MatchPropertyAlerts, HandlePropertyAlerts },
            { MatchPostCodeSearch, HandlePostCodeSearch },
            { MatchAddressSearch, HandleAddressSearch },
            { MatchPropertyReference, HandlePropertyReference }
        };

        private static ApiResponse<object> HandleTenancyInformation(string value)
        {
            ListTenanciesApiResponse result = new ListTenanciesApiResponse
            {
                Tenancies = MockTenantApiResponses.Where(tenant => tenant.PropertyReference == value).ToList()
            };

            return BuildResponse<object>(result);
        }

        private static RouteParams MatchTenancyInformation(string url)
        {
            return new RouteParams
            {
                Matches = url.Contains("testtenanciesapi"),
                QueryValue = url.Split("=").Last()
            };
        }

        private static RouteParams MatchPersonAlerts(string url)
        {
            return new RouteParams
            {
                Matches = url.Contains("testalertsapi") && url.Contains("people?tag_ref"),
                QueryValue = url.Split("=").Last()
            };
        }

        private static ApiResponse<object> HandlePersonAlerts(string value)
        {
            ListPersonAlertsApiResponse result = new ListPersonAlertsApiResponse
            {
                Contacts = MockPersonAlertsApiResponses.Where(res => res.TenancyAgreementReference == value).ToList()
            };

            return BuildResponse<object>(result);
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

        private static RouteParams MatchPropertyAlerts(string url)
        {
            return new RouteParams
            {
                Matches = url.Contains("testalertsapi") && url.Contains("properties"),
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

        private static ApiResponse<object> HandlePropertyAlerts(string value)
        {
            return BuildResponse<object>(MockPropertyAlertsApiResponses.Where(res => res.PropertyReference == value).FirstOrDefault());
        }
        #endregion

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

        internal static PropertyApiResponse NewProperty()
        {
            var newProperty = StubPropertyApiResponse().Generate();

            MockPropertyApiResponses.Add(newProperty);

            return newProperty;
        }

        internal static void AddPropertyAlerts(int count, string propRef)
        {
            PropertyAlertsApiResponse mockAlerts = StubPropertyAlertApiResponse(count, propRef).Generate();
            MockPropertyAlertsApiResponses.Add(mockAlerts);
        }

        internal static void AddTenantInformation(string tenantReference, string propRef, bool canRaiseRepair = false)
        {
            string code = canRaiseRepair ? ApiModelFactory.RaisableTenureCodes.First() : "not a valid code";
            MockTenantApiResponses.Add(new TenancyApiTenancyInformation
            {
                TenancyAgreementReference = tenantReference,
                PropertyReference = propRef,
                TenureType = $"{code}: Rnadom descirption"
            });
        }

        internal static void AddPersonAlerts(int count, string tenantReferance)
        {
            PersonAlertsApiResponse mockAlerts = StubPersonAlertApiResponse(count, tenantReferance).Generate();

            MockPersonAlertsApiResponses.Add(mockAlerts);
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
