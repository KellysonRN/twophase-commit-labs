using TwoPhaseCommit.Labs.Domain.Entities;
using TwoPhaseCommit.Labs.Domain.Interfaces;

namespace TwoPhaseCommit.Labs.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public Task<Order?> GetByIdAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
