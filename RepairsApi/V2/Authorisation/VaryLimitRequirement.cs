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

            var updatedCodes = resource.MoreSpecificSORCode.RateScheduleItem
                .Select(rsi => new { rsi.CustomCode, Amount = rsi.Quantity.Amount.Sum() }).ToList();

            var originalCodes = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem).Where(rsi => rsi.Original).Select(rsi => new { rsi.OriginalQuantity, rsi.CustomCode });

            double totalCost = 0;
            await updatedCodes.ForEachAsync(async c => totalCost += c.Amount * await _sorGateway.GetCost(contractorRef, c.CustomCode));

            double originalCost = 0;
            await originalCodes.ForEachAsync(async c => originalCost += c.OriginalQuantity.Value * await _sorGateway.GetCost(contractorRef, c.CustomCode));

            var limit = double.Parse(context.User.FindFirst(CustomClaimTypes.VaryLimit).Value);

            if (totalCost <= originalCost + limit)
            {
                context.Succeed(requirement);
            }
        }
    }
}
