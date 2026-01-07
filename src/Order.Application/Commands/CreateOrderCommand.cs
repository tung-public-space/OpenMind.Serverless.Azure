using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Commands;

/// <summary>
/// Command to create a new order
/// </summary>
public record CreateOrderCommand : IRequest<OrderDto>
{
    public string CustomerId { get; init; } = string.Empty;
    public AddressDto ShippingAddress { get; init; } = null!;
    public AddressDto? BillingAddress { get; init; }
    public List<CreateOrderItemCommand> Items { get; init; } = new();
}

public record CreateOrderItemCommand
{
    public string ProductId { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string Currency { get; init; } = "USD";
}

