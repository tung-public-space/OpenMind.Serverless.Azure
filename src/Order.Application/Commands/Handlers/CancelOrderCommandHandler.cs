using MediatR;
using Order.Application.Exceptions;
using Order.Domain.Repositories;

namespace Order.Application.Commands.Handlers;

/// <summary>
/// Handler for CancelOrderCommand
/// </summary>
public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        order.Cancel(request.Reason);

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return true;
    }
}

