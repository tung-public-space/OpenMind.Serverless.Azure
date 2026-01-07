using MediatR;
using Order.Application.DTOs;
using Order.Application.Mappers;
using Order.Domain.Repositories;

namespace Order.Application.Queries.Handlers;

/// <summary>
/// Handler for GetOrderByIdQuery
/// </summary>
public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        return order?.ToDto();
    }
}

