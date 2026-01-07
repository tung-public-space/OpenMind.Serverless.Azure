namespace Order.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of an order
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}

