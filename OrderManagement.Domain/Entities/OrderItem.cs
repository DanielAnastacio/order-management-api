using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public class OrderItem
{
    // Used by EF Core for entity materialization.
    private OrderItem()
    {
    }

    public OrderItem(
        string productName,
        int quantity,
        decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException(
                "Product name is required.");

        if (quantity <= 0)
            throw new DomainException(
                "Quantity must be greater than zero.");

        if (unitPrice <= 0)
            throw new DomainException(
                "Unit price must be greater than zero.");

        Id = Guid.NewGuid();
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Total =>
        Quantity * UnitPrice;

    /// <summary>
    /// Associates this item with its owning order.
    /// </summary>
    internal void AssignToOrder(Guid orderId)
    {
        if (orderId == Guid.Empty)
            throw new DomainException(
                "Order id is required.");

        OrderId = orderId;
    }
}