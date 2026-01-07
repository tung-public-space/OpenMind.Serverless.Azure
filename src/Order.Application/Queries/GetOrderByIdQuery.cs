using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Queries;

/// <summary>
/// Query to get an order by ID
/// </summary>
public record GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid OrderId { get; init; }
}

