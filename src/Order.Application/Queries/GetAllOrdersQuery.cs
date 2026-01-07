using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Queries;

/// <summary>
/// Query to get all orders with pagination
/// </summary>
public record GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>
{
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 100;
}

