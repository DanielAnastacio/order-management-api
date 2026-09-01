using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Orders.DTOs;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items);