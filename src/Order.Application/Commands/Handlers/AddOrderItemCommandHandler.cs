using MediatR;
using Order.Application.DTOs;
using Order.Application.Exceptions;
using Order.Application.Mappers;
using Order.Domain.Repositories;
using Order.Domain.ValueObjects;

namespace Order.Application.Commands.Handlers;

/// <summary>
/// Handler for AddOrderItemCommand
/// </summary>
public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public AddOrderItemCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        var unitPrice = new Money(request.UnitPrice, request.Currency);
        order.AddItem(request.ProductId, request.ProductName, request.Quantity, unitPrice);

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return order.ToDto();
    }
}

