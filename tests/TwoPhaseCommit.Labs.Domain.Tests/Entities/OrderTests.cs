using FluentAssertions;
using TwoPhaseCommit.Labs.Domain.Entities;
using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void New_order_should_start_as_pending()
    {
        var order = new Order("order-1");

        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void Order_can_be_activated_from_pending()
    {
        var order = new Order("order-1");

        order.Activate();

        order.Status.Should().Be(OrderStatus.Active);
    }

    [Fact]
    public void Order_cannot_be_activated_if_not_pending()
    {
        var order = new Order("order-1");
        order.Fail();

        Action act = () => order.Activate();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Order_can_fail_from_pending()
    {
        var order = new Order("order-1");

        order.Fail();

        order.Status.Should().Be(OrderStatus.Failed);
    }

    [Fact]
    public void Order_cannot_fail_if_not_pending()
    {
        var order = new Order("order-1");
        order.Activate();

        Action act = () => order.Fail();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Order_should_contain_added_items()
    {
        var order = new Order("order-1");
        var item = new Item("item-1");

        order.AddItem(item);

        order.Items.Should().Contain(item);
    }
}
