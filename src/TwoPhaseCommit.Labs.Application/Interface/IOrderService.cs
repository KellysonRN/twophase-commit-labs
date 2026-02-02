using TwoPhaseCommit.Labs.Domain.Entities;

namespace TwoPhaseCommit.Labs.Application.Interface;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default);
}
