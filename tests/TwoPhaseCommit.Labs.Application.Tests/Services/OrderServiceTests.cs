using FluentAssertions;
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

        await _service.CreateOrderAsync(order);

        Assert.Contains(State.Pending, savedStatuses);
        Assert.Contains(State.Active, savedStatuses);

        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Exactly(2));

    }

    [Fact]
    public async Task CreateOrder_should_fail_order_if_repository_throws()
    {
        var orderId = Guid.NewGuid();

        var order = Order.Create(orderId);
        order.AddItem(OrdemItem.Create(Guid.NewGuid()));

        var savedStatuses = new List<State>();

        _repositoryMock
           .Setup(r => r.SaveAsync(It.IsAny<Order>()))
           .Callback<Order>(o => savedStatuses.Add(o.Status))
           .ThrowsAsync(new Exception("db error"));

        Func<Task> act = () => _service.CreateOrderAsync(order);

        await act.Should().ThrowAsync<Exception>();

        Assert.Contains(State.Pending, savedStatuses);
        Assert.Contains(State.Failed, savedStatuses);

        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateOrder_should_save_pending_before_active()
    {
        var orderId = Guid.NewGuid();

        var order = Order.Create(orderId);
        order.AddItem(OrdemItem.Create(Guid.NewGuid()));

        var callSequence = new List<State>();

        _repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<Order>()))
            .Callback<Order>(o => callSequence.Add(o.Status))
            .Returns(Task.CompletedTask);

        await _service.CreateOrderAsync(order);

        callSequence.Should().ContainInOrder(
            State.Pending,
            State.Active);
    }

    [Fact]
    public async Task CreateOrder_should_not_activate_if_initial_save_fails()
    {
        var orderId = Guid.NewGuid();

        var order = Order.Create(orderId);
        order.AddItem(OrdemItem.Create(Guid.NewGuid()));

        _repositoryMock
            .SetupSequence(r => r.SaveAsync(It.IsAny<Order>()))
            .ThrowsAsync(new Exception("fail"))
            .Returns(Task.CompletedTask);

        Func<Task> act = () => _service.CreateOrderAsync(order);

        await act.Should().ThrowAsync<Exception>();

        _repositoryMock.Verify(
            r => r.SaveAsync(It.Is<Order>(o => o.Status == State.Active)),
            Times.Never);
    }
}
