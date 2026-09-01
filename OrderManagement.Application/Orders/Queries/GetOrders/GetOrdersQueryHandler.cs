using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Orders.DTOs;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler
    : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderDto>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _orderRepository.CountAsync(
            cancellationToken);

        var items = orders
            .Select(order => new OrderDto(
                order.Id,
                order.CustomerId,
                order.Status,
                order.CreatedAt,
                order.TotalAmount,
                order.Items
                    .Select(item => new OrderItemDto(
                        item.Id,
                        item.ProductName,
                        item.Quantity,
                        item.UnitPrice,
                        item.Total))
                    .ToList()))
            .ToList();

        return new PagedResult<OrderDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount);
    }
}