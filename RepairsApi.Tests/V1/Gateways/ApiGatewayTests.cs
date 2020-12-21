using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V1.Gateways;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.Gateways
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "They are disposed by the DI system in prod")]
    public class ApiGatewayTests
    {
        [Test]
        public async Task ReturnDeserialisedResult()
        {
            TestModel expectedResponse = new TestModel
            {
                TestValue = "Dummy Text"
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
            };

            ApiGateway classUnderTest = SetUpHttpMock(response);

            var result = await classUnderTest.ExecuteRequest<TestModel>(new Uri("http://test"));

            result.Content.Should().BeEquivalentTo(expectedResponse);
        }

        private static ApiGateway SetUpHttpMock(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);
            return new ApiGateway(new HttpClientFactoryWrapper(httpClient));
        }
    }

    public class TestModel
    {
        public string TestValue { get; set; }
    }
}
