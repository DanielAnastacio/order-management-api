using Moq;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Orders.Queries.GetOrders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Orders.Queries;

public sealed class GetOrdersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPagedOrders()
    {
        // Arrange
        var repository = new Mock<IOrderRepository>();

        var orders = new[]
        {
            CreateOrder(),
            CreateOrder()
        };

        repository
            .Setup(x => x.GetPagedAsync(
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        repository
            .Setup(x => x.CountAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var handler =
            new GetOrdersQueryHandler(repository.Object);

        // Act
        var result = await handler.Handle(
            new GetOrdersQuery(1, 10),
            CancellationToken.None);

        // Assert
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
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