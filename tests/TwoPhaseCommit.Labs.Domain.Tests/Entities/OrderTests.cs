using FluentAssertions;
using TwoPhaseCommit.Labs.Domain.Entities;
using TwoPhaseCommit.Labs.Domain.Exceptions;
using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void New_order_should_start_as_pending()
    {
        var order = Order.Create(Guid.NewGuid());

        order.Status.Should().Be(State.Pending);
    }

    [Fact]
    public void Order_can_be_activated_from_pending()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(OrdemItem.Create(Guid.NewGuid()));

        order.Activate();

        order.Status.Should().Be(State.Active);
    }

    [Fact]
    public void Order_cannot_be_activated_if_not_pending()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(OrdemItem.Create(Guid.NewGuid()));

        order.Fail();

        Action act = () => order.Activate();

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Order_can_fail_from_pending()
    {
        var order = Order.Create(Guid.NewGuid());

        order.Fail();

        order.Status.Should().Be(State.Failed);
    }

    [Fact]
    public void Order_cannot_fail_if_not_pending()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(OrdemItem.Create(Guid.NewGuid()));

        order.Activate();

        Action act = () => order.Fail();

        act.Should().Throw<InvalidStateTransitionException>();
    }

    [Fact]
    public void Order_should_contain_added_items()
    {
        var order = Order.Create(Guid.NewGuid());
        var item = OrdemItem.Create(Guid.NewGuid());

        order.AddItem(item);

        order.Items.Should().Contain(item);
    }

    [Fact]
    public void Order_should_be_marked_as_failed_when_any_item_fails()
    {
        var order = Order.Create(Guid.NewGuid());
        var item1 = OrdemItem.Create(Guid.NewGuid());
        var item2 = OrdemItem.Create(Guid.NewGuid());

        order.AddItem(item1);
        order.AddItem(item2);

        item2.Fail();
        order.Fail();

        order.Status.Should().Be(State.Failed);
    }

    [Fact]
    public void Order_should_not_be_activated_if_any_item_is_failed()
    {

        var order = Order.Create(Guid.NewGuid());
        var item = OrdemItem.Create(Guid.NewGuid());

        order.AddItem(item);
        item.Fail();

        Action act = () => order.Activate();

        act.Should().Throw<BusinessRuleViolationException>()
           .WithMessage("Order cannot be activated if any item has failed.");
    }

    [Fact]
    public void Item_should_not_be_activated_when_parent_order_is_failed()
    {

        var order = Order.Create(Guid.NewGuid());
        var item = OrdemItem.Create(Guid.NewGuid());

        order.AddItem(item);
        order.Fail();

        Action act = () => order.Activate();

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Failed_order_should_not_transition_back_to_active()
    {
        var order = Order.Create(Guid.NewGuid());
        order.Fail();

        Action act = () => order.Activate();

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Order_should_not_be_activated_without_items()
    {
        var order = Order.Create(Guid.NewGuid());

        Action act = () => order.Activate();

        act.Should().Throw<BusinessRuleViolationException>()
           .WithMessage("*at least one item*");
    }

    [Fact]
    public void Marking_failed_order_as_failed_again_should_be_idempotent()
    {
        var order = Order.Create(Guid.NewGuid());
        order.Fail();

        order.Fail();

        order.Status.Should().Be(State.Failed);
    }
}
