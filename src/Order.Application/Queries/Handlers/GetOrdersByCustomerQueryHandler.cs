using MediatR;
using Order.Application.DTOs;
using Order.Application.Mappers;
using Order.Domain.Repositories;

namespace Order.Application.Queries.Handlers;

/// <summary>
/// Handler for GetOrdersByCustomerQuery
/// </summary>
public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return orders.Select(o => o.ToDto());
    }
}

