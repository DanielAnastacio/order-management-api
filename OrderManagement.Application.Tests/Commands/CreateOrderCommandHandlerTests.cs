using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Orders.Commands;

public sealed class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateOrderAndReturnItsId()
    {
        // Arrange
        var repository = new Mock<IOrderRepository>();

        Order? capturedOrder = null;

        repository
            .Setup(x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>(
                (order, _) => capturedOrder = order)
            .Returns(Task.CompletedTask);

        repository
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler =
            new CreateOrderCommandHandler(repository.Object);

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new[]
            {
                new CreateOrderItemCommand(
                    "Notebook",
                    1,
                    5000m),

                new CreateOrderItemCommand(
                    "Mouse",
                    2,
                    150m)
            });

        // Act
        var orderId = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, orderId);

        Assert.NotNull(capturedOrder);
        Assert.Equal(orderId, capturedOrder.Id);
        Assert.Equal(2, capturedOrder.Items.Count);
        Assert.Equal(5300m, capturedOrder.TotalAmount);

        repository.Verify(
            x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}