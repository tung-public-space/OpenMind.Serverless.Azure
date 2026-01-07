using MediatR;
using Order.Application.DTOs;
using Order.Application.Exceptions;
using Order.Application.Mappers;
using Order.Domain.Repositories;

namespace Order.Application.Commands.Handlers;

/// <summary>
/// Handler for UpdateOrderStatusCommand
/// </summary>
public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        switch (request.Action.ToLowerInvariant())
        {
            case "confirm":
                order.Confirm();
                break;
            case "ship":
                order.Ship();
                break;
            case "deliver":
                order.Deliver();
                break;
            case "cancel":
                order.Cancel(request.Reason ?? "No reason provided");
                break;
            default:
                throw new InvalidOperationException($"Unknown action: {request.Action}");
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return order.ToDto();
    }
}

