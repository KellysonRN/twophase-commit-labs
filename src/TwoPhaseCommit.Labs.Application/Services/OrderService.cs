using TwoPhaseCommit.Labs.Application.Interface;
using TwoPhaseCommit.Labs.Domain.Entities;
using TwoPhaseCommit.Labs.Domain.Interfaces;

namespace TwoPhaseCommit.Labs.Application.Services;

public class OrderService(IOrderRepository orderRepository) : IOrderService
{
    public async Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            await orderRepository.SaveAsync(order);

            order.Activate();

            await orderRepository.SaveAsync(order);

            return order;
        }
        catch
        {
            order.Fail();
            await orderRepository.SaveAsync(order);
            throw;
        }
    }
}
