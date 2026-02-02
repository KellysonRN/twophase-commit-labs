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


        await _service.CreateOrderAsync(order);
             
        _repositoryMock.Verify(
            r => r.SaveAsync(It.Is<Order>(o => o.Status == State.Pending)),
            Times.Once);

        _repositoryMock.Verify(
            r => r.SaveAsync(It.Is<Order>(o => o.Status == State.Active)),
            Times.Once);
    }
}
