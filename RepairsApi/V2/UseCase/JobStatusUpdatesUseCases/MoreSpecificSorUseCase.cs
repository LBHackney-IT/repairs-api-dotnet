using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class MoreSpecificSorUseCase : IMoreSpecificSorUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly RepairsContext _repairsContext;

        public MoreSpecificSorUseCase(IRepairsGateway repairsGateway, RepairsContext repairsContext)
        {
            _repairsGateway = repairsGateway;
            _repairsContext = repairsContext;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);
            var workElement = jobStatusUpdate.MoreSpecificSORCode.ToDb();

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (workOrder == null)
            {
                throw new ResourceNotFoundException($"Unable to locate work order {workOrderId}");
            }

            var existingCodes = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem);
            var newCodes = workElement.RateScheduleItem.Where(rsi => !existingCodes.Any(ec => ec.CustomCode == rsi.CustomCode));

            UpdateExistingCodes(existingCodes, workElement);
            AddNewCodes(newCodes, workOrder);

            await _repairsContext.SaveChangesAsync();
        }

        private static void AddNewCodes(IEnumerable<RateScheduleItem> newCodes, WorkOrder workOrder)
        {

            foreach (var newCode in newCodes)
            {
                workOrder.WorkElements.First().RateScheduleItem.Add(newCode);
            }
        }

        private static void UpdateExistingCodes(IEnumerable<RateScheduleItem> existingCodes, WorkElement workElement)
        {

            foreach (var existingCode in existingCodes)
            {
                var updatedCode = workElement.RateScheduleItem.SingleOrDefault(rsi => rsi.CustomCode == existingCode.CustomCode);
                if (updatedCode == null)
                {
                    throw new NotSupportedException($"Deleting SOR codes not supported, missing {existingCode}");
                }

                existingCode.Quantity.Amount = updatedCode.Quantity.Amount;
            }
        }
    }

}
