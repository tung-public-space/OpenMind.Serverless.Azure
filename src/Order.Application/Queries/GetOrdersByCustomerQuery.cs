using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Queries;

/// <summary>
/// Query to get orders by customer ID
/// </summary>
public record GetOrdersByCustomerQuery : IRequest<IEnumerable<OrderDto>>
{
    public string CustomerId { get; init; } = string.Empty;
}

