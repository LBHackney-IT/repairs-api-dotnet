using Microsoft.AspNetCore.Mvc;
using Moq;
using RepairsApi.Tests.ApiMocking;
using RepairsApi.Tests.Helpers;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using static RepairsApi.Tests.V1.DataFakers;

namespace RepairsApi.Tests
{
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Parameters Map to query param values for platform apis")]
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

        private readonly ApiGateway _innerGateway;

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Tests")]
        public MockApiGateway()
        {
            var mock = MockHttpMessageHandler.FromClass<MockApiGateway>();
            HttpClient client = new HttpClient(mock);
            var factoryMock = new HttpClientFactoryWrapper(client);
            
            _innerGateway = new ApiGateway(factoryMock);
        }

        public Task<ApiResponse<TResponse>> ExecuteRequest<TResponse>(Uri url) where TResponse : class
        {
            return _innerGateway.ExecuteRequest<TResponse>(url);
        }

        [Route("http://testtenanciesapi/tenancies")]
        public static object HandleTenancyInformation(string property_reference)
        {
            return new ListTenanciesApiResponse
            {
                Tenancies = MockTenantApiResponses.Where(tenant => tenant.PropertyReference == property_reference).ToList()
            };
        }

        [Route("http://testalertsapi/cautionary-alerts/people")]
        public static object HandlePersonAlerts(string tag_ref)
        {
            return new ListPersonAlertsApiResponse
            {
                Contacts = MockPersonAlertsApiResponses.Where(res => res.TenancyAgreementReference == tag_ref).ToList()
            };
        }

        [Route("http://testalertsapi/cautionary-alerts/properties/{propertyReference}")]
        public static object HandlePropertyAlerts(string propertyReference)
        {
            return MockPropertyAlertsApiResponses.Where(res => res.PropertyReference == propertyReference).FirstOrDefault();
        }

        [Route("http://testpropertiesapi/properties/{propertyReference}")]
        public static object HandlePropertyReference(string propertyReference)
        {
            return MockPropertyApiResponses.Where(res => res.PropRef == propertyReference).FirstOrDefault();
        }

        [Route("http://testpropertiesapi/properties")]
        public static object HandlePostCodeSearch(string address, string postcode)
        {
            return MockPropertyApiResponses
                .Where(prop => (!string.IsNullOrEmpty(postcode) && prop.PostCode.Equals(postcode))
                            || (!string.IsNullOrEmpty(address) && prop.Address1.Contains(address)))
                .ToList();
        }

        public static object HandleAddressSearch(string value)
        {
            return MockPropertyApiResponses.Where(prop => prop.Address1.Contains(value)).ToList();
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
    }
}
