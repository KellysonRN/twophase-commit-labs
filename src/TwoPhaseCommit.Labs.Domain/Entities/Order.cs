using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Domain.Entities;

public sealed class Order
{
    public Guid OrderId { get; private set; }

    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<Item> Items => _items;

    private readonly List<Item> _items = [];

    public Order() { }

    public static Order Create(Guid id)
    {
        return new Order
        {
            OrderId = id,
            Status = OrderStatus.Pending
        };
    }

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
