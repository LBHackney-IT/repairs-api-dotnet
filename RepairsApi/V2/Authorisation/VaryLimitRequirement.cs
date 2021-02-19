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
    public class VaryLimitRequirement : IAuthorizationRequirement
    {
    }

    public class VarySpendLimitAuthorizationHandler :
        AuthorizationHandler<VaryLimitRequirement, JobStatusUpdate>
    {
        private readonly IScheduleOfRatesGateway _sorGateway;
        private readonly IRepairsGateway _repairsGateway;

        public VarySpendLimitAuthorizationHandler(
            IScheduleOfRatesGateway sorGateway,
            IRepairsGateway repairsGateway
            )
        {
            _sorGateway = sorGateway;
            _repairsGateway = repairsGateway;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            VaryLimitRequirement requirement,
            JobStatusUpdate resource
            )
        {
            var workOrderId = int.Parse(resource.RelatedWorkOrderReference.ID);
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            var contractorRef = workOrder.AssignedToPrimary.ContractorReference;

            var rawCodes = resource.MoreSpecificSORCode.RateScheduleItem
                .Select(rsi => (code: rsi.CustomCode, amount: rsi.Quantity.Amount.Sum())).ToList();

            rawCodes.AddRange(workOrder.WorkElements
                .SelectMany(we => we.RateScheduleItem)
                .Where(rsi => !rsi.Original)
                .Select(rsi => (code: rsi.CustomCode, amount: rsi.Quantity.Amount)));

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
