using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.Authorisation
{
    public class RaiseLimitRequirement : IAuthorizationRequirement
    {
    }

    public class RaiseSpendLimitAuthorizationHandler :
        AuthorizationHandler<RaiseLimitRequirement, ScheduleRepair>
    {
        private readonly IScheduleOfRatesGateway _sorGateway;

        public RaiseSpendLimitAuthorizationHandler(IScheduleOfRatesGateway sorGateway)
        {
            _sorGateway = sorGateway;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RaiseLimitRequirement requirement,
            ScheduleRepair resource
            )
        {
            var contractorRef = resource.AssignedToPrimary.Reference?.FirstOrDefault()?.ID;
            var rawCodes = resource.WorkElement
                .SelectMany(we => we.RateScheduleItem)
                .Select(rsi => (code: rsi.CustomCode, amount: rsi.Quantity.Amount.Sum()));
            double totalCost = 0;
            await rawCodes.ForEachAsync(async c => totalCost += c.amount * await _sorGateway.GetCost(contractorRef, c.code) ?? 0);

            var limit = double.Parse(context.User.FindFirst(CustomClaimTypes.RAISELIMIT).Value);

            if (totalCost <= limit)
            {
                context.Succeed(requirement);
            }
        }
    }
}
