using MediatR;
using Order.Application.DTOs;
using Order.Application.Mappers;
using Order.Domain.Repositories;
using Order.Domain.ValueObjects;

namespace Order.Application.Commands.Handlers;

/// <summary>
/// Handler for CreateOrderCommand
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var shippingAddress = request.ShippingAddress.ToDomain();
        var billingAddress = request.BillingAddress?.ToDomain();

        var order = Domain.Entities.Order.Create(
            request.CustomerId,
            shippingAddress,
            billingAddress);

        foreach (var item in request.Items)
        {
            var unitPrice = new Money(item.UnitPrice, item.Currency);
            order.AddItem(item.ProductId, item.ProductName, item.Quantity, unitPrice);
        }

        await _orderRepository.AddAsync(order, cancellationToken);

        return order.ToDto();
    }
}

