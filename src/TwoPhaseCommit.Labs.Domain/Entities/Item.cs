using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Domain.Entities;

public class Item
{
    public string ItemId { get; }

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public Item(string itemId) => ItemId = itemId;
    
    public void Activate() => Status = OrderStatus.Active;

    public void Fail() => Status = OrderStatus.Failed;
}
