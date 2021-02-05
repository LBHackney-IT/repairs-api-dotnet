using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Services;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class ListWorkOrderNotesUseCase : IListWorkOrderNotesUseCase
    {
        private readonly IRepairsGateway _repairsGatway;

        public ListWorkOrderNotesUseCase(IRepairsGateway repairsGatway)
        {
            _repairsGatway = repairsGatway;
        }

        public async Task<IEnumerable<NoteListItem>> Execute(int id)
        {
            var workOrder = await _repairsGatway.GetWorkOrder(id);

            if (workOrder == null)
            {
                throw new ResourceNotFoundException($"Unable to find work order: {id}");
            }

            return workOrder.JobStatusUpdates?
                .Where(jsu => !jsu.Comments.IsNullOrEmpty())
                .Where(jsu => jsu.EventTime.HasValue)
                .OrderBy(jsu => jsu.EventTime)
                .Select(jsu => new NoteListItem
                {
                    Note = jsu.Comments,
                    Time = jsu.EventTime.Value,
                    User = jsu.Author
                }).ToList();
        }
    }
}
