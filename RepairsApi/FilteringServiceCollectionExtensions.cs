using Microsoft.Extensions.DependencyInjection;
using RepairsApi.V2.Controllers.Parameters;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Infrastructure;
using System;
using System.Linq;

namespace RepairsApi
{
    public static class FilteringServiceCollectionExtensions
    {
        public static void AddFilteringConfig(this IServiceCollection services)
        {
            services.AddFilter<WorkOrderSearchParameters, WorkOrder>(filter =>
            {
                filter.AddFilter(
                    searchParams => searchParams.ContractorReference,
                    contractorReference => !string.IsNullOrWhiteSpace(contractorReference),
                    contractorReference => wo => wo.AssignedToPrimary.ContractorReference == contractorReference
                )
                .AddFilter(
                    searchParams => searchParams.PropertyReference,
                    p => !string.IsNullOrWhiteSpace(p),
                    p => wo => wo.Site.PropertyClass.Any(pc => pc.PropertyReference == p)
                )
                .AddFilter(
                    searchParams => searchParams.StatusCodes,
                    codes => codes?.All(code => code > 0 && Enum.IsDefined(typeof(WorkStatusCode), code)) ?? false,
                    codes => wo => codes.Select(c => Enum.Parse<WorkStatusCode>(c.ToString())).Contains(wo.StatusCode)
                );
            });
        }

        public static void AddFilter<TSearch, TQuery>(this IServiceCollection services, Action<FilterBuilder<TSearch, TQuery>> options)
        {
            services.AddSingleton<IFilterBuilder<TSearch, TQuery>>(sp =>
            {
                FilterBuilder<TSearch, TQuery> builder = new FilterBuilder<TSearch, TQuery>();
                options(builder);
                return builder;
            });
        }
    }
}
