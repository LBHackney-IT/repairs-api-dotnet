using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class UpdateSorCodesUseCase : IUpdateSorCodesUseCase
    {
        public Task Execute(WorkOrder workOrder, WorkElement workElement)
        {
            var existingCodes = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem).ToList();
            var newCodes = workElement.RateScheduleItem.Where(rsi => !existingCodes.Any(ec => ec.Id == rsi.OriginalId)).ToList();

            UpdateExistingCodes(existingCodes, workElement);
            workOrder.WorkElements.First().RateScheduleItem.AddRange(newCodes);

            return Task.CompletedTask;
        }

        private static void UpdateExistingCodes(IEnumerable<RateScheduleItem> existingCodes, WorkElement workElement)
        {

            foreach (var existingCode in existingCodes)
            {
                var updatedCode = workElement.RateScheduleItem.SingleOrDefault(rsi => rsi.OriginalId == existingCode.Id);
                if (updatedCode == null)
                {
                    throw new NotSupportedException($"Deleting SOR codes not supported, missing {existingCode.CustomCode}");
                }

                existingCode.Quantity.Amount = updatedCode.Quantity.Amount;
            }
        }
    }

    public interface IUpdateSorCodesUseCase
    {
        Task Execute(WorkOrder workOrder, WorkElement workElement);
    }
}
