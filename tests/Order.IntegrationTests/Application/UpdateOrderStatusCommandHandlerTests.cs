using FluentAssertions;
using Order.Application.Commands;
using Order.Application.Commands.Handlers;
using Order.Application.DTOs;
using Order.Application.Exceptions;
using Order.Domain.Enums;
using Order.Infrastructure.Repositories;
using Xunit;

namespace Order.IntegrationTests.Application;

public class UpdateOrderStatusCommandHandlerTests
{
    private readonly InMemoryOrderRepository _repository;
    private readonly CreateOrderCommandHandler _createHandler;
    private readonly UpdateOrderStatusCommandHandler _updateHandler;

    public UpdateOrderStatusCommandHandlerTests()
    {
        _repository = new InMemoryOrderRepository();
        _createHandler = new CreateOrderCommandHandler(_repository);
        _updateHandler = new UpdateOrderStatusCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_ConfirmAction_ShouldConfirmOrder()
    {
        // Arrange
        var order = await CreateTestOrder();
        var command = new UpdateOrderStatusCommand
        {
            OrderId = order.Id,
            Action = "confirm"
        };

        // Act
        var result = await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_ShipAction_ShouldShipConfirmedOrder()
    {
        // Arrange
        var order = await CreateTestOrder();
        await _updateHandler.Handle(new UpdateOrderStatusCommand
        {
            OrderId = order.Id,
            Action = "confirm"
        }, CancellationToken.None);

        var command = new UpdateOrderStatusCommand
        {
            OrderId = order.Id,
            Action = "ship"
        };

        // Act
        var result = await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public async Task Handle_CancelAction_ShouldCancelOrder()
    {
        // Arrange
        var order = await CreateTestOrder();
        var command = new UpdateOrderStatusCommand
        {
            OrderId = order.Id,
            Action = "cancel",
            Reason = "Customer changed their mind"
        };

        // Act
        var result = await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateOrderStatusCommand
        {
            OrderId = Guid.NewGuid(),
            Action = "confirm"
        };

        // Act
        var act = async () => await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OrderNotFoundException>();
    }

    private async Task<OrderDto> CreateTestOrder()
    {
        var command = new CreateOrderCommand
        {
            CustomerId = "customer-123",
            ShippingAddress = new AddressDto
            {
                Street = "123 Main St",
                City = "Seattle",
                State = "WA",
                PostalCode = "98101",
                Country = "USA"
            },
            Items = new List<CreateOrderItemCommand>
            {
                new CreateOrderItemCommand
                {
                    ProductId = "product-1",
                    ProductName = "Test Product",
                    Quantity = 1,
                    UnitPrice = 29.99m,
                    Currency = "USD"
                }
            }
        };

        return await _createHandler.Handle(command, CancellationToken.None);
    }
}

