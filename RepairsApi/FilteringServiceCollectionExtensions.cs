using Microsoft.Extensions.DependencyInjection;
using RepairsApi.V2.Controllers.Parameters;
using RepairsApi.V2.Enums;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Extensions;
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
                    contractorReference => contractorReference?.Count > 0,
                    contractorReference => wo => contractorReference.Contains(wo.AssignedToPrimary.ContractorReference)
                )
                .AddFilter(
                    searchParams => searchParams.TradeCodes,
                    tc => tc?.Count > 0,
                    tc => wo => tc.Contains(wo.WorkElements.FirstOrDefault().Trade.FirstOrDefault().CustomCode)
                )
                .AddFilter(
                    searchParams => searchParams.PropertyReference,
                    p => !string.IsNullOrWhiteSpace(p),
                    p => wo => wo.Site.PropertyClass.Any(pc => pc.PropertyReference == p)
                )
                .AddFilter(
                    searchParams => searchParams.StatusCode,
                    codes => codes?.All(code => code > 0 && Enum.IsDefined(typeof(WorkStatusCode), code)) ?? false,
                    codes => wo => codes.Select(c => Enum.Parse<WorkStatusCode>(c.ToString())).Contains(wo.StatusCode)
                )
                .AddFilter(
                    searchParams => searchParams.Priorities,
                    codes => codes?.Count > 0,
                    codes => wo => wo.WorkPriority.PriorityCode.HasValue && codes.Contains(wo.WorkPriority.PriorityCode.Value)
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
