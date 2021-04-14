using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Controllers.Parameters;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class ListWorkOrdersUseCase : IListWorkOrdersUseCase
    {
        private readonly IRepairsGateway _repairsGateway;

        public ListWorkOrdersUseCase(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task<IEnumerable<WorkOrderListItem>> Execute(WorkOrderSearchParameters searchParameters)
        {
            IEnumerable<WorkOrder> workOrders = await _repairsGateway.GetWorkOrders(GetConstraints(searchParameters));

            var statusOrder = new[] {
                WorkOrderStatus.InProgress,
                WorkOrderStatus.PendApp,
                WorkOrderStatus.Cancelled,
                WorkOrderStatus.Complete,
                WorkOrderStatus.Unknown
            };

            return workOrders.Select(wo => wo.ToListItem())
                .OrderBy(wo => Array.IndexOf(statusOrder, wo.Status))
                .ThenByDescending(wo => wo.DateRaised)
                .Skip((searchParameters.PageNumber - 1) * searchParameters.PageSize)
                .Take(searchParameters.PageSize)
                .ToList();
        }

        private static Expression<Func<WorkOrder, bool>>[] GetConstraints(WorkOrderSearchParameters searchParameters)
        {
            var result = new List<Expression<Func<WorkOrder, bool>>>();

            if (!string.IsNullOrWhiteSpace(searchParameters.PropertyReference))
            {
                result.Add(wo => wo.Site.PropertyClass.Any(pc => pc.PropertyReference == searchParameters.PropertyReference));
            }

            if (!string.IsNullOrWhiteSpace(searchParameters.ContractorReference))
            {
                result.Add(wo => wo.AssignedToPrimary.ContractorReference == searchParameters.ContractorReference);
            }

            if (searchParameters.StatusCode > 0 && Enum.IsDefined(typeof(WorkStatusCode), searchParameters.StatusCode))
            {
                result.Add(wo => wo.StatusCode == (WorkStatusCode) Enum.Parse(typeof(WorkStatusCode), searchParameters.StatusCode.ToString()));
            }

            var filterBuilder = Test();

            var filter = filterBuilder.BuildFilter(searchParameters);

            return result.ToArray();
        }

        public static FilterBuilder<WorkOrderSearchParameters, WorkOrder> Test()
        {
            return new FilterBuilder<WorkOrderSearchParameters, WorkOrder>()
                .AddFilter(
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
                    searchParams => searchParams.StatusCode,
                    code => code > 0 && Enum.IsDefined(typeof(WorkStatusCode), code),
                    code => wo => wo.StatusCode == (WorkStatusCode)Enum.Parse(typeof(WorkStatusCode), code.ToString())
                );
        }
    }

    public class FilterBuilder<TSearch, TQuery>
    {
        private List<IFilterItem<TSearch, TQuery>> _config = new List<IFilterItem<TSearch, TQuery>>();

        internal FilterBuilder<TSearch, TQuery> AddFilter<T>(Func<TSearch, T> searchValueFunction, Func<T, bool> searchValidator, Func<T, Expression<Func<TQuery, bool>>> filterFunction)
        {
           _config.Add(new FilterItem<T, TSearch, TQuery>(searchValueFunction, searchValidator, filterFunction));
           return this;
        }

        internal IFilter<TQuery> BuildFilter(TSearch searchParameter)
        {
            return new Filter<TQuery>(_config.Where(c => c.IsValid(searchParameter)).Select(c => c.CreateExpression(searchParameter)));
        }
    }

    public class Filter<TQuery> : IFilter<TQuery>
    {
        private IEnumerable<Expression<Func<TQuery, bool>>> _predicates;

        public Filter(IEnumerable<Expression<Func<TQuery, bool>>> predicates)
        {
            _predicates = predicates;
        }

        public IQueryable<TQuery> Apply(IQueryable<TQuery> query)
        {
            return _predicates.Aggregate(query, (q, pred) => q.Where(pred));
        }
    }

    public interface IFilter<TQuery>
    {
        IQueryable<TQuery> Apply(IQueryable<TQuery> query);
    }

    public class FilterItem<T, TSearch, TQuery> : IFilterItem<TSearch, TQuery>
    {
        private Func<TSearch, T> _searchValueFunction;
        private readonly Func<T, bool> _searchValidator;
        private Func<T, Expression<Func<TQuery, bool>>> _filterFunction;

        public FilterItem(Func<TSearch, T> p, Func<T, bool> searchValidator, Func<T, Expression<Func<TQuery, bool>>> predicate)
        {
            this._searchValueFunction = p;
            _searchValidator = searchValidator;
            this._filterFunction = predicate;
        }

        public Expression<Func<TQuery, bool>> CreateExpression(TSearch search)
        {
            T value = _searchValueFunction(search);

            var filterExpression = _filterFunction(value);

            return filterExpression;
        }

        public bool IsValid(TSearch search)
        {
            T value = _searchValueFunction(search);
            return _searchValidator(value);
        }
    }

    public interface IFilterItem<TSearch, TQuery>
    {
        Expression<Func<TQuery, bool>> CreateExpression(TSearch search);

        bool IsValid(TSearch search);
    }
}
