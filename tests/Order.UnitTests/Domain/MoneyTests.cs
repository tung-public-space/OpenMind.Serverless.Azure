using FluentAssertions;
using Order.Domain.ValueObjects;
using Xunit;

namespace Order.UnitTests.Domain;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidValues_ShouldSucceed()
    {
        // Act
        var money = new Money(100.99m, "USD");

        // Assert
        money.Amount.Should().Be(100.99m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Act
        var act = () => new Money(-10m, "USD");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Amount cannot be negative*");
    }

    [Fact]
    public void Create_WithEmptyCurrency_ShouldThrowException()
    {
        // Act
        var act = () => new Money(10m, "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency is required*");
    }

    [Fact]
    public void Addition_ShouldAddAmounts()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Addition_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        // Act
        var act = () => money1 + money2;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot operate on different currencies*");
    }

    [Fact]
    public void Multiplication_ShouldMultiplyAmount()
    {
        // Arrange
        var money = new Money(25.00m, "USD");

        // Act
        var result = money * 4;

        // Assert
        result.Amount.Should().Be(100.00m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Zero_ShouldReturnZeroAmount()
    {
        // Act
        var money = Money.Zero("EUR");

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("EUR");
    }
}

