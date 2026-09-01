using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = [];

    // Used by EF Core for entity materialization.
    private Order()
    {
    }

    private Order(Guid customerId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount =>
        _items.Sum(item => item.Total);

    public static Order Create(
        Guid customerId,
        IEnumerable<OrderItem> items)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Customer id is required.");

        var itemList = items?.ToList() ?? [];

        if (itemList.Count == 0)
            throw new DomainException(
                "An order must contain at least one item.");

        var order = new Order(customerId);

        foreach (var item in itemList)
        {
            item.AssignToOrder(order.Id);
            order._items.Add(item);
        }

        return order;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException(
                "Only pending orders can be cancelled.");

        Status = OrderStatus.Cancelled;
    }
}