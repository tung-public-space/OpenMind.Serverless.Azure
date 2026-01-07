using FluentAssertions;
using Order.Application.Commands;
using Order.Application.Commands.Handlers;
using Order.Application.DTOs;
using Order.Domain.Enums;
using Order.Infrastructure.Repositories;
using Xunit;

namespace Order.IntegrationTests.Application;

public class CreateOrderCommandHandlerTests
{
    private readonly InMemoryOrderRepository _repository;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _repository = new InMemoryOrderRepository();
        _handler = new CreateOrderCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateOrder()
    {
        // Arrange
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
                    Quantity = 2,
                    UnitPrice = 29.99m,
                    Currency = "USD"
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CustomerId.Should().Be("customer-123");
        result.Status.Should().Be(OrderStatus.Pending);
        result.Items.Should().ContainSingle();
        result.TotalAmount.Should().Be(59.98m);
    }

    [Fact]
    public async Task Handle_ShouldPersistOrder()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = "customer-456",
            ShippingAddress = new AddressDto
            {
                Street = "456 Oak Ave",
                City = "Portland",
                State = "OR",
                PostalCode = "97201",
                Country = "USA"
            },
            Items = new List<CreateOrderItemCommand>
            {
                new CreateOrderItemCommand
                {
                    ProductId = "product-2",
                    ProductName = "Another Product",
                    Quantity = 1,
                    UnitPrice = 49.99m,
                    Currency = "USD"
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var persistedOrder = await _repository.GetByIdAsync(result.Id);
        persistedOrder.Should().NotBeNull();
        persistedOrder!.CustomerId.Should().Be("customer-456");
    }
}

