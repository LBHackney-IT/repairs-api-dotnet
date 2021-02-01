using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Gateways.Models;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using static RepairsApi.Tests.V2.DataFakers;

namespace RepairsApi.Tests.V2.Gateways
{
    public class PropertyGatewayTests
    {
        private Mock<IApiGateway> _apiGatewayMock;
        private PropertyGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _apiGatewayMock = new Mock<IApiGateway>();
            _classUnderTest = new PropertyGateway(_apiGatewayMock.Object, new NullLogger<PropertyGateway>());
        }

        [Test]
        public async Task SendsRequestById()
        {
            // Arrange
            var stubData = BuildResponse(StubPropertyApiResponse().Generate());
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<PropertyApiResponse>(It.IsAny<string>(), It.IsAny<Uri>())).ReturnsAsync(stubData);

            // Act
            var result = await _classUnderTest.GetByReferenceAsync("");

            // Assert
            result.Address.ShortAddress.Should().Be(stubData.Content.Address1);
        }

        [Test]
        public async Task SendsRequestBySearch()
        {
            // Arrange
            var stubData = BuildResponse(StubPropertyApiResponse().Generate(5));
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<List<PropertyApiResponse>>(It.IsAny<string>(), It.IsAny<Uri>())).ReturnsAsync(stubData);
            PropertySearchModel searchModel = new PropertySearchModel
            {
            };

            // Act
            var result = await _classUnderTest.GetByQueryAsync(searchModel);

            // Assert
            result.Should().HaveCount(stubData.Content.Count);
        }

        [Test]
        public async Task ThrowsFor404()
        {
            // Arrange
            var stubData = BuildResponse<PropertyApiResponse>(null);
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<PropertyApiResponse>(It.IsAny<string>(), It.IsAny<Uri>())).ReturnsAsync(stubData);

            // Act
            Func<Task> actFunction = async () => await _classUnderTest.GetByReferenceAsync("");

            // Assert
            await actFunction.Should().ThrowAsync<ResourceNotFoundException>();
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
