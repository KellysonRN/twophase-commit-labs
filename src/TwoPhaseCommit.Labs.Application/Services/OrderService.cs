using TwoPhaseCommit.Labs.Application.Interface;
using TwoPhaseCommit.Labs.Domain.Entities;
using TwoPhaseCommit.Labs.Domain.Interfaces;

namespace TwoPhaseCommit.Labs.Application.Services;

public class OrderService(IOrderRepository orderRepository) : IOrderService
{
    public Task CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
