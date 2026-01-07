namespace Order.Application.Exceptions;

/// <summary>
/// Exception thrown when an order is not found
/// </summary>
public class OrderNotFoundException : Exception
{
    public Guid OrderId { get; }

    public OrderNotFoundException(Guid orderId)
        : base($"Order with ID '{orderId}' was not found.")
    {
        OrderId = orderId;
    }
}

