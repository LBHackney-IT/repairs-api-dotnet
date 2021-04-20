using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    public class MockRepairsGateway : Mock<IRepairsGateway>
    {
        private IEnumerable<WorkOrder> _workOrders = new List<WorkOrder>();
        private int _workOrderId;
        public WorkOrder LastWorkOrder { get; set; }

        public MockRepairsGateway()
        {
            Setup(g => g.GetWorkOrder(It.IsAny<int>()))
                .ReturnsAsync((int id) => _workOrders.SingleOrDefault(wo => wo.Id == id));

            Setup(m => m.CreateWorkOrder(It.IsAny<WorkOrder>()))
                .Callback<WorkOrder>(wo => LastWorkOrder = wo)
                .ReturnsAsync(() => _workOrderId);

            Setup(g => g.GetWorkOrders(It.IsAny<Expression<Func<WorkOrder, bool>>[]>()))
                .ReturnsAsync((Expression<Func<WorkOrder, bool>>[] expressions) =>
                {
                    var tempWorkOrders = _workOrders;
                    foreach (var whereExpression in expressions)
                    {
                        tempWorkOrders = tempWorkOrders.Where(whereExpression.Compile());
                    }
                    return tempWorkOrders;
                });

            Setup(g => g.GetWorkOrders(It.IsAny<IFilter<WorkOrder>>()))
                .ReturnsAsync((IFilter<WorkOrder> filter) =>
                {
                    var tempWorkOrders = _workOrders;
                    tempWorkOrders = filter.Apply(tempWorkOrders);
                    return tempWorkOrders;
                });
        }

        public void ReturnsWorkOrders(IEnumerable<WorkOrder> workOrders)
        {
            _workOrders = workOrders;


        }

        public void ReturnWOId(int newId)
        {
            _workOrderId = newId;
        }
    }
}
