using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Commands;

/// <summary>
/// Command to add an item to an existing order
/// </summary>
public record AddOrderItemCommand : IRequest<OrderDto>
{
    public Guid OrderId { get; init; }
    public string ProductId { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string Currency { get; init; } = "USD";
}

