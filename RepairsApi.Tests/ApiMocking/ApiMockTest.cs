using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace RepairsApi.Tests.ApiMocking
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
    public class StaticApiMockTest
    {
        [Test]
        public async Task ApiMockerWorksWithStaticMethods()
        {
            var mock = MockHttpMessageHandler.FromClass<StaticApiMockTest>();

            HttpClient client = new HttpClient(mock);

            var result = await client.GetAsync(new Uri("http://test/routetest?query=querytest"));

            var stringResult = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<TestResult>(stringResult);

            response.Route.Should().Be("routetest");
            response.Query.Should().Be("querytest");

            mock.Dispose();
        }

        [Test]
        public async Task ApiMockerWorksWithInstancedMethods()
        {
            var mock = MockHttpMessageHandler.FromObject(new InstancedApiMockTest("testinstance"));

            HttpClient client = new HttpClient(mock);

            var result = await client.GetAsync(new Uri("http://test/routetest?query=querytest"));

            var stringResult = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<TestResult>(stringResult);

            response.Route.Should().Be("routetest");
            response.Query.Should().Be("querytest");
            response.Instance.Should().Be("testinstance");

            mock.Dispose();
        }

        [Route("http://test/{fromRoute}")]
        public static TestResult TestMethod(string fromRoute, string query)
        {
            return new TestResult
            {
                Route = fromRoute,
                Query = query
            };
        }
    }

    public class InstancedApiMockTest
    {
        private string _instanceString;

        public InstancedApiMockTest(string instanceString)
        {
            _instanceString = instanceString;
        }

        [Route("http://test/{fromRoute}")]
        public TestResult TestMethod(string fromRoute, string query)
        {
            return new TestResult
            {
                Route = fromRoute,
                Query = query,
                Instance = _instanceString
            };
        }
    }

    public class TestResult
    {
        public string Instance { get; set; }
        public string Query { get; set; }
        public string Route { get; set; }
    }
}
