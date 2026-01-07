using FluentAssertions;
using Order.Domain.Enums;
using Order.Domain.Events;
using Order.Domain.ValueObjects;
using Xunit;

namespace Order.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_ShouldCreateOrderWithPendingStatus()
    {
        // Arrange
        var customerId = "customer-123";
        var shippingAddress = new Address("123 Main St", "Seattle", "WA", "98101", "USA");

        // Act
        var order = Order.Domain.Entities.Order.Create(customerId, shippingAddress);

        // Assert
        order.Should().NotBeNull();
        order.Id.Should().NotBeEmpty();
        order.CustomerId.Should().Be(customerId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.ShippingAddress.Should().Be(shippingAddress);
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        // Arrange
        var customerId = "customer-123";
        var shippingAddress = new Address("123 Main St", "Seattle", "WA", "98101", "USA");

        // Act
        var order = Order.Domain.Entities.Order.Create(customerId, shippingAddress);

        // Assert
        order.DomainEvents.Should().ContainSingle();
        order.DomainEvents.First().Should().BeOfType<OrderCreatedEvent>();
    }

    [Fact]
    public void AddItem_ShouldAddItemAndUpdateTotal()
    {
        // Arrange
        var order = CreateTestOrder();
        var unitPrice = new Money(29.99m, "USD");

        // Act
        order.AddItem("product-1", "Test Product", 2, unitPrice);

        // Assert
        order.Items.Should().ContainSingle();
        order.Items.First().ProductId.Should().Be("product-1");
        order.Items.First().Quantity.Should().Be(2);
        order.TotalAmount.Amount.Should().Be(59.98m);
    }

    [Fact]
    public void AddItem_ShouldUpdateQuantityForExistingProduct()
    {
        // Arrange
        var order = CreateTestOrder();
        var unitPrice = new Money(10.00m, "USD");
        order.AddItem("product-1", "Test Product", 2, unitPrice);

        // Act
        order.AddItem("product-1", "Test Product", 3, unitPrice);

        // Assert
        order.Items.Should().ContainSingle();
        order.Items.First().Quantity.Should().Be(5);
        order.TotalAmount.Amount.Should().Be(50.00m);
    }

    [Fact]
    public void Confirm_ShouldChangeStatusToConfirmed()
    {
        // Arrange
        var order = CreateTestOrderWithItems();

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Confirm_WithEmptyOrder_ShouldThrowException()
    {
        // Arrange
        var order = CreateTestOrder();

        // Act
        var act = () => order.Confirm();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot confirm an empty order");
    }

    [Fact]
    public void Ship_ShouldChangeStatusToShipped()
    {
        // Arrange
        var order = CreateTestOrderWithItems();
        order.Confirm();

        // Act
        order.Ship();

        // Assert
        order.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public void Ship_WithPendingOrder_ShouldThrowException()
    {
        // Arrange
        var order = CreateTestOrderWithItems();

        // Act
        var act = () => order.Ship();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only confirmed orders can be shipped");
    }

    [Fact]
    public void Deliver_ShouldChangeStatusToDelivered()
    {
        // Arrange
        var order = CreateTestOrderWithItems();
        order.Confirm();
        order.Ship();

        // Act
        order.Deliver();

        // Assert
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var order = CreateTestOrderWithItems();

        // Act
        order.Cancel("Customer request");

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_DeliveredOrder_ShouldThrowException()
    {
        // Arrange
        var order = CreateTestOrderWithItems();
        order.Confirm();
        order.Ship();
        order.Deliver();

        // Act
        var act = () => order.Cancel("Customer request");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot cancel a delivered or already cancelled order");
    }

    private static Order.Domain.Entities.Order CreateTestOrder()
    {
        var shippingAddress = new Address("123 Main St", "Seattle", "WA", "98101", "USA");
        return Order.Domain.Entities.Order.Create("customer-123", shippingAddress);
    }

    private static Order.Domain.Entities.Order CreateTestOrderWithItems()
    {
        var order = CreateTestOrder();
        order.AddItem("product-1", "Test Product", 2, new Money(29.99m, "USD"));
        return order;
    }
}

