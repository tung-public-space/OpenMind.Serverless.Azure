using Order.Domain.Enums;
using Order.Domain.Events;
using Order.Domain.ValueObjects;

namespace Order.Domain.Entities;

/// <summary>
/// Order aggregate root
/// </summary>
public class Order
{
    private readonly List<OrderItem> _items = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; private set; }
    public string CustomerId { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public Address? BillingAddress { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Order() { } // For EF Core

    public static Order Create(
        string customerId,
        Address shippingAddress,
        Address? billingAddress = null)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress ?? shippingAddress,
            Status = OrderStatus.Pending,
            TotalAmount = Money.Zero("USD"),
            CreatedAt = DateTime.UtcNow
        };

        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerId));
        return order;
    }

    public void AddItem(string productId, string productName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending order");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(OrderItem.Create(productId, productName, quantity, unitPrice));
        }

        RecalculateTotal();
        AddDomainEvent(new OrderItemAddedEvent(Id, productId, quantity));
    }

    public void RemoveItem(string productId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from a non-pending order");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
            AddDomainEvent(new OrderItemRemovedEvent(Id, productId));
        }
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm an empty order");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderConfirmedEvent(Id));
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped");

        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderShippedEvent(Id));
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered");

        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderDeliveredEvent(Id));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a delivered or already cancelled order");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderCancelledEvent(Id, reason));
    }

    private void RecalculateTotal()
    {
        var currency = _items.FirstOrDefault()?.UnitPrice.Currency ?? "USD";
        var total = _items.Sum(i => i.TotalPrice.Amount);
        TotalAmount = new Money(total, currency);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}

