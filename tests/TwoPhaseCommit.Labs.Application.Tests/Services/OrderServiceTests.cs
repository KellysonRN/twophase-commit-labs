using Moq;
using TwoPhaseCommit.Labs.Application.Services;
using TwoPhaseCommit.Labs.Domain.Entities;
using TwoPhaseCommit.Labs.Domain.Interfaces;
using TwoPhaseCommit.Labs.Domain.ValueObjects;

namespace TwoPhaseCommit.Labs.Application.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;

    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _service = new OrderService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateOrder_should_persist_and_activate_order()
    {        
        var orderId = Guid.NewGuid();

        var order = Order.Create(orderId);
        order.AddItem(OrdemItem.Create(Guid.NewGuid()));

        var savedStatuses = new List<State>();

        _repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<Order>()))
            .Callback<Order>(o => savedStatuses.Add(o.Status))
            .Returns(Task.CompletedTask);

        var orderCreated = await _service.CreateOrderAsync(order);

        Assert.Contains(State.Pending, savedStatuses);
        Assert.Contains(State.Active, savedStatuses);

        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Exactly(2));

    }
}
