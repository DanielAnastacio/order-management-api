using MediatR;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItemCommand> Items)
    : IRequest<Guid>;

public sealed record CreateOrderItemCommand(
    string ProductName,
    int Quantity,
    decimal UnitPrice);