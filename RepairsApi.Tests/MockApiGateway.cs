using RepairsApi.V1.Gateways;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static RepairsApi.Tests.V1.DataFakers;

namespace RepairsApi.Tests
{
    public class MockApiGateway : IApiGateway
    {
        public static List<AlertsApiResponse> AlertsApiResponse => StubAlertApiResponse().Generate(20);
        public static List<PropertyApiResponse> PropertyApiResponse => StubPropertyApiResponse().Generate(40);

        public Task<TResponse> ExecuteRequest<TResponse>(Uri url) where TResponse : class
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

        private static Task<TResponse> HandlePropertyRequest<TResponse>(string urlString) where TResponse : class
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

        private static Task<TResponse> HandleRequestByReference<TResponse>(string urlString) where TResponse : class
        {
            string propRef = urlString.Split("/").Last();

            return Task.FromResult(PropertyApiResponse.Where(res => res.PropRef == propRef) as TResponse);
        }

        private static Task<TResponse> HandleRequestBySearch<TResponse>(string urlString) where TResponse : class
        {
            Regex searchExtractor = new Regex(@"\?(([a-z]+)=(\w*))");
            var groups = searchExtractor.Match(urlString).Groups;
            string param = groups[1].Value;
            string value = groups[2].Value;

            if (param == "address")
            {
                return Task.FromResult(PropertyApiResponse.Where(prop => prop.Address1.Contains(value)) as TResponse);
            }
            else if (param == "postcode")
            {
                return Task.FromResult(PropertyApiResponse.Where(prop => prop.PostCode.Equals(value)) as TResponse);
            }

            return null;
        }

        private static Task<TResponse> HandleAlertRequest<TResponse>(string urlString) where TResponse : class
        {
            string propRef = urlString.Split("/").Last();

            return Task.FromResult(AlertsApiResponse.Where(res => res.PropertyReference == propRef) as TResponse);
        }
    }
}
