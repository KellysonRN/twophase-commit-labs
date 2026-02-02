using TwoPhaseCommit.Labs.Domain.Entities;

namespace TwoPhaseCommit.Labs.Application.Interface;

public interface IOrderService
{
    Task CreateOrderAsync(Order order, CancellationToken cancellationToken = default);


    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default);
}
