using Moq;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Orders.Queries;

public sealed class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var repository = new Mock<IOrderRepository>();

        var order = CreateOrder();

        repository
            .Setup(x => x.GetByIdAsync(
                order.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler =
            new GetOrderByIdQueryHandler(repository.Object);

        // Act
        var result = await handler.Handle(
            new GetOrderByIdQuery(order.Id),
            CancellationToken.None);

        // Assert
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.CustomerId, result.CustomerId);
        Assert.Equal(5300m, result.TotalAmount);
        Assert.Equal(2, result.Items.Count);
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
            new GetOrderByIdQueryHandler(repository.Object);

        // Act
        var action = () => handler.Handle(
            new GetOrderByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    private static Order CreateOrder()
    {
        var items = new[]
        {
            new OrderItem(
                "Notebook",
                1,
                5000m),

            new OrderItem(
                "Mouse",
                2,
                150m)
        };

        return Order.Create(
            Guid.NewGuid(),
            items);
    }
}