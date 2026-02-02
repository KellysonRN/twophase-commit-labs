using System.Collections.ObjectModel;
using TwoPhaseCommit.Labs.Domain.Entities;

namespace TwoPhaseCommit.Labs.Domain.Interfaces;

public interface IOrderItemRepository
{
    Task<ReadOnlyCollection<OrdemItem>> GetByOrderIdAsync(Guid orderId);

    Task SaveAsync(OrdemItem orderItem);
}
