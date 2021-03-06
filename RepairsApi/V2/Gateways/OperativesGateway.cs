using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Infrastructure;


namespace RepairsApi.V2.Gateways
{
    public class OperativesGateway : IOperativesGateway
    {
        private readonly RepairsContext _context;

        public OperativesGateway(RepairsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Operative>> ListByFilterAsync(IFilter<Operative> filter)
        {
            var query = filter.Apply(_context.Operatives);
            return await query.ToListAsync();
        }

        public async Task<Operative> GetAsync(string operativePrn)
        {
            var query = _context.Operatives
                .IgnoreQueryFilters()
                .Where(operative => operative.PayrollNumber == operativePrn);
            return await query.SingleOrDefaultAsync();
        }

        public async Task ArchiveAsync(string operativePrn)
        {
            var operative = await _context.Operatives.SingleOrDefaultAsync(o => o.PayrollNumber == operativePrn);
            if (operative is null) ThrowHelper.ThrowNotFound($"Operative with payroll {operativePrn} not found");

            _context.Remove(operative);
            await _context.SaveChangesAsync();
        }

        public async Task AssignOperatives(int workOrderId, OperativeAssignmentType type, params int[] operativeIds)
        {
            var workOrderOperatives = _context.WorkOrderOperatives.Where(woo => woo.WorkOrderId == workOrderId).AsEnumerable();
            _context.WorkOrderOperatives.RemoveRange(workOrderOperatives);
            var assignments = operativeIds.Select(o => new WorkOrderOperative
            {
                OperativeId = o,
                WorkOrderId = workOrderId,
                AssignmentType = type
            });
            await _context.WorkOrderOperatives.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();
        }
    }
}
