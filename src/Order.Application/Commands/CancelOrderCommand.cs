using MediatR;

namespace Order.Application.Commands;

/// <summary>
/// Command to cancel an order
/// </summary>
public record CancelOrderCommand : IRequest<bool>
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

