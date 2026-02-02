using TwoPhaseCommit.Labs.Domain.Entities;

namespace TwoPhaseCommit.Labs.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);

    Task SaveAsync(Order order);
}
