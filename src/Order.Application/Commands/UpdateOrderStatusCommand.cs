using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Commands;

/// <summary>
/// Command to update order status
/// </summary>
public record UpdateOrderStatusCommand : IRequest<OrderDto>
{
    public Guid OrderId { get; init; }
    public string Action { get; init; } = string.Empty; // confirm, ship, deliver, cancel
    public string? Reason { get; init; } // For cancellation
}

