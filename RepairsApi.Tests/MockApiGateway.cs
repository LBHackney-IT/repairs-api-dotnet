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
    public class MockApiGateway : IApiGateway
    {
        private static List<AlertsApiResponse> _alertsApiResponse;
        private static List<PropertyApiResponse> _propertyApiResponse;

        public static List<AlertsApiResponse> AlertsApiResponse
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

        public static List<PropertyApiResponse> PropertyApiResponse
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

            if (urlString.Contains("testalertsapi"))
            {
                return HandleAlertRequest<TResponse>(urlString);
            }
            else if (urlString.Contains("testpropertiesapi"))
            {
                return HandlePropertyRequest<TResponse>(urlString);
            }
            else
            {
                return null;
            }
        }

        private static Task<ApiResponse<TResponse>> HandlePropertyRequest<TResponse>(string urlString) where TResponse : class
        {
            if (urlString.Contains("?"))
            {
                return HandleRequestBySearch<TResponse>(urlString);
            }
            else
            {
                return HandleRequestByReference<TResponse>(urlString);
            }
        }

        private static Task<ApiResponse<TResponse>> HandleRequestByReference<TResponse>(string urlString) where TResponse : class
        {
            string propRef = urlString.Split("/").Last();

            ApiResponse<TResponse> response = BuildResponse(PropertyApiResponse.Where(res => res.PropRef == propRef).FirstOrDefault() as TResponse);
            return Task.FromResult(response);
        }

        private static Task<ApiResponse<TResponse>> HandleRequestBySearch<TResponse>(string urlString) where TResponse : class
        {
            Regex searchExtractor = new Regex(@"\?(([a-z]+)=(\w*))");
            var groups = searchExtractor.Match(urlString).Groups;
            string param = groups[2].Value;
            string value = groups[3].Value;

            if (param == "address")
            {
                ApiResponse<TResponse> response = BuildResponse(PropertyApiResponse.Where(prop => prop.Address1.Contains(value)).ToList() as TResponse);
                return Task.FromResult(response);
            }
            else if (param == "postcode")
            {
                ApiResponse<TResponse> response = BuildResponse(PropertyApiResponse.Where(prop => prop.PostCode.Equals(value)).ToList() as TResponse);
                return Task.FromResult(response);
            }

            return null;
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

            PropertyApiResponse.AddRange(newProperties);
        }

        private static Task<ApiResponse<TResponse>> HandleAlertRequest<TResponse>(string urlString) where TResponse : class
        {
            string propRef = urlString.Split("/").Last();

            ApiResponse<TResponse> response = BuildResponse(AlertsApiResponse.Where(res => res.PropertyReference == propRef).FirstOrDefault() as TResponse);
            return Task.FromResult(response);
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
}
