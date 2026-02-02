using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Domain.Entities;

public sealed class Order
{
    public string OrderId { get; private set; }

    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<Item> Items => _items;

    private readonly List<Item> _items = [];

    public Order(string orderId) => OrderId = orderId;

    public void AddItem(Item item) => _items.Add(item);

    public void Activate()
    {
        EnsurePending();
        Status = OrderStatus.Active;
    }

    public void Fail()
    {
        EnsurePending();
        Status = OrderStatus.Failed;
    }

    private void EnsurePending()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Invalid state transition");
    }
}
