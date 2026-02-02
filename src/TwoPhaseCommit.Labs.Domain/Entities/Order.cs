using TwoPhaseCommit.Labs.Domain.Exceptions;
using TwoPhaseCommit.Labs.Domain.Interfaces;
using TwoPhaseCommit.Labs.Domain.Policies;
using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Domain.Entities;

public sealed class Order
{
    public Guid OrderId { get; private set; }

    public State Status { get; private set; } = State.Pending;

    public IReadOnlyCollection<OrdemItem> Items => _items;

    private readonly List<OrdemItem> _items = [];

    private readonly IStateTransitionPolicy<State> _stateTransitionPolicy;

    public Order(IStateTransitionPolicy<State> stateTransitionPolicy) => _stateTransitionPolicy = stateTransitionPolicy;

    public static Order Create(Guid id)
    {
        return new Order(new StatePolicy())
        {
            OrderId = id
        };
    }

    public void AddItem(OrdemItem item)
    {
        if (Status != State.Pending)
            throw new BusinessRuleViolationException(
                "Items can only be added while the order is pending.");

        _items.Add(item);
    }

    public void Activate()
    {
        if (!_items.Any())
            throw new BusinessRuleViolationException(
                "An order must have at least one item to be activated.");

        if (_items.Any(i => i.IsFailed()))
            throw new BusinessRuleViolationException(
                "Order cannot be activated if any item has failed.");

        ChangeStatus(State.Active);
    }

    public void ActivateItem(Guid itemId)
    {
        if (Status == State.Failed)
            throw new BusinessRuleViolationException(
                "Cannot activate items when the order has failed.");

        var item = _items.Single(i => i.OrdemItemId == itemId);

        item.Activate();
    }

    public void Fail()
    {
        ChangeStatus(State.Failed);

        foreach (var item in _items)
            item.Fail();
    }

    public bool IsFailed()
      => Status == State.Failed;

    private void ChangeStatus(State newStatus)
    {
        if (!_stateTransitionPolicy.CanTransition(Status, newStatus))
            throw new InvalidStateTransitionException(
                nameof(Order),
                Status.ToString(),
                newStatus.ToString());

        

        Status = newStatus;
    }
}
