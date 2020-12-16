using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.E2ETests
{
    public class PropertyApitests : IntegrationTests<Startup>
    {
        [Test]
        public async Task GetSingleProperty()
        {
            var property = MockApiGateway.PropertyApiResponse.First();

            var result = await Client.GetAsync(new Uri($"/api/v1/properties/{property.PropRef}", UriKind.Relative)).ConfigureAwait(false);
        }
    }
}
