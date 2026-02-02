using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Domain.Entities;

public class Item
{
    public Guid ItemId { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public Item() { }

    public static Item Create(Guid itemId)
    {
        return new Item
        {
            ItemId = itemId,
            Status = OrderStatus.Pending
        };
    }  

    public void Activate() => Status = OrderStatus.Active;

    public void Fail() => Status = OrderStatus.Failed;
}
