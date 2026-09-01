using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var items = request.Items
            .Select(item => new OrderItem(
                item.ProductName,
                item.Quantity,
                item.UnitPrice))
            .ToList();

        var order = Order.Create(
            request.CustomerId,
            items);

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return order.Id;
    }
}