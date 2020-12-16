using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.Gateways.Models;
using RepairsApi.V1.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.Gateways
{
    public class PropertyGatewayTests
    {
        private Mock<IApiGateway> _apiGatewayMock;
        private PropertyGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            var gatewayOptions = new GatewayOptions
            {
                AlertsApi = new Uri("http://test"),
                PropertiesAPI = new Uri("http://test"),
            };

            _apiGatewayMock = new Mock<IApiGateway>();
            _classUnderTest = new PropertyGateway(Options.Create(gatewayOptions), _apiGatewayMock.Object);
        }

        [Test]
        public async Task SendsRequestById()
        {
            // Arrange
            PropertyApiResponse stubData = StubPropertyApiResponse().Generate();
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<PropertyApiResponse>(It.IsAny<Uri>())).ReturnsAsync(stubData);

            // Act
            var result = await _classUnderTest.GetByReferenceAsync("").ConfigureAwait(false);

            // Assert
            result.Address.ShortAddress.Should().Be(stubData.Address1);
        }

        [Test]
        public async Task SendsRequestBySearch()
        {
            // Arrange
            List<PropertyApiResponse> stubData = StubPropertyApiResponse().Generate(5);
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<List<PropertyApiResponse>>(It.IsAny<Uri>())).ReturnsAsync(stubData);
            PropertySearchModel searchModel = new PropertySearchModel
            {
            };

            // Act
            var result = await _classUnderTest.GetByQueryAsync(searchModel).ConfigureAwait(false);

            // Assert
            result.Should().HaveCount(stubData.Count);
        }

        private static Faker<PropertyApiResponse> StubPropertyApiResponse()
        {
            return new Faker<PropertyApiResponse>()
                .RuleFor(res => res.Address1, f => f.Random.String())
                .RuleFor(res => res.PostCode, f => f.Random.String())
                .RuleFor(res => res.LevelCode, f => f.Random.String())
                .RuleFor(res => res.PropRef, f => f.Random.Int().ToString())
                .RuleFor(res => res.SubtypCode, f => f.PickRandom<string>(ApiModelFactory.HierarchyDescriptions.Keys));
        }
    }
}
