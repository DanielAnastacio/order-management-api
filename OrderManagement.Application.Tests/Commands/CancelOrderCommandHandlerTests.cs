using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Application.Tests.Orders.Commands;

public sealed class CancelOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCancelPendingOrder()
    {
        // Arrange
        var repository = new Mock<IOrderRepository>();

        var order = CreateOrder();

        repository
            .Setup(x => x.GetByIdAsync(
                order.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        repository
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler =
            new CancelOrderCommandHandler(repository.Object);

        var command = new CancelOrderCommand(order.Id);

        // Act
        await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.Equal(
            OrderStatus.Cancelled,
            order.Status);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var repository = new Mock<IOrderRepository>();

        repository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler =
            new CancelOrderCommandHandler(repository.Object);

        // Act
        var action = () => handler.Handle(
            new CancelOrderCommand(Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task Handle_ShouldThrowDomainException_WhenOrderIsAlreadyCancelled()
    {
        // Arrange
        var repository = new Mock<IOrderRepository>();

        var order = CreateOrder();

        order.Cancel();

        repository
            .Setup(x => x.GetByIdAsync(
                order.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler =
            new CancelOrderCommandHandler(repository.Object);

        // Act
        var action = () => handler.Handle(
            new CancelOrderCommand(order.Id),
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<DomainException>(action);
    }

    private static Order CreateOrder()
    {
        var item = new OrderItem(
            "Notebook",
            1,
            5000m);

        return Order.Create(
            Guid.NewGuid(),
            new[] { item });
    }
}