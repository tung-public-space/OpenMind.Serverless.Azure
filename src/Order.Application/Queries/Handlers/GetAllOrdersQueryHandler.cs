using MediatR;
using Order.Application.DTOs;
using Order.Application.Mappers;
using Order.Domain.Repositories;

namespace Order.Application.Queries.Handlers;

/// <summary>
/// Handler for GetAllOrdersQuery
/// </summary>
public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(request.Skip, request.Take, cancellationToken);
        return orders.Select(o => o.ToDto());
    }
}

