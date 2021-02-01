using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    public class MockRepairsGateway : Mock<IRepairsGateway>
    {
        private IEnumerable<WorkOrder> _workOrders;

        public MockRepairsGateway()
        {
            Setup(g => g.GetWorkOrder(It.IsAny<int>()))
                .ReturnsAsync((int id) => _workOrders.SingleOrDefault(wo => wo.Id == id));
        }

        public void ReturnsWorkOrders(List<WorkOrder> workOrders)
        {
            _workOrders = workOrders;

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
        }
    }
}