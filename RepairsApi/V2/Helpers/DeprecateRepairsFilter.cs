using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RepairsApi.V2.Helpers
{
    public class DeprecateRepairsFilter : IOperationFilter
    {
        private const string DeprecatedMessage = "Deprecated please use endpoints under /WorkOrders";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.RelativePath.EndsWith("repairs") ||
                context.ApiDescription.RelativePath.Contains("/repairs/"))
            {
                operation.Deprecated = true;
                operation.Summary += $" ({DeprecatedMessage})";
                foreach (var openApiTag in operation.Tags)
                {
                    openApiTag.Name = $"Repairs ({DeprecatedMessage})";
                }
            }
        }
    }
}
