using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Authorisation
{
    public class RaiseLimitRequirement : IAuthorizationRequirement
    {
    }

    public class RaiseSpendLimitAuthorizationHandler :
        AuthorizationHandler<RaiseLimitRequirement, WorkOrder>
    {
        private readonly IScheduleOfRatesGateway _sorGateway;

        public RaiseSpendLimitAuthorizationHandler(IScheduleOfRatesGateway sorGateway)
        {
            _sorGateway = sorGateway;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RaiseLimitRequirement requirement,
            WorkOrder resource
            )
        {
            var contractorRef = resource.AssignedToPrimary?.ContractorReference;
            var rawCodes = resource.WorkElements
                .SelectMany(we => we.RateScheduleItem)
                .Select(rsi => new { rsi.CustomCode, Amount = rsi.Quantity.Amount });
            double totalCost = 0;
            await rawCodes.ForEachAsync(async c => totalCost += c.Amount * await _sorGateway.GetCost(contractorRef, c.CustomCode));

            var limit = double.Parse(context.User.FindFirst(CustomClaimTypes.RAISELIMIT).Value);

            if (totalCost <= limit)
            {
                context.Succeed(requirement);
            }
        }
    }
}
