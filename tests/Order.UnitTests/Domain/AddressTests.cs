using FluentAssertions;
using Order.Domain.ValueObjects;
using Xunit;

namespace Order.UnitTests.Domain;

public class AddressTests
{
    [Fact]
    public void Create_WithValidValues_ShouldSucceed()
    {
        // Act
        var address = new Address("123 Main St", "Seattle", "WA", "98101", "USA");

        // Assert
        address.Street.Should().Be("123 Main St");
        address.City.Should().Be("Seattle");
        address.State.Should().Be("WA");
        address.PostalCode.Should().Be("98101");
        address.Country.Should().Be("USA");
    }

    [Fact]
    public void Create_WithEmptyStreet_ShouldThrowException()
    {
        // Act
        var act = () => new Address("", "Seattle", "WA", "98101", "USA");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Street is required*");
    }

    [Fact]
    public void Create_WithEmptyCity_ShouldThrowException()
    {
        // Act
        var act = () => new Address("123 Main St", "", "WA", "98101", "USA");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("City is required*");
    }

    [Fact]
    public void Create_WithEmptyCountry_ShouldThrowException()
    {
        // Act
        var act = () => new Address("123 Main St", "Seattle", "WA", "98101", "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Country is required*");
    }

    [Fact]
    public void FullAddress_ShouldReturnFormattedAddress()
    {
        // Arrange
        var address = new Address("123 Main St", "Seattle", "WA", "98101", "USA");

        // Act
        var fullAddress = address.FullAddress;

        // Assert
        fullAddress.Should().Be("123 Main St, Seattle, WA 98101, USA");
    }

    [Fact]
    public void Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Seattle", "WA", "98101", "USA");
        var address2 = new Address("123 Main St", "Seattle", "WA", "98101", "USA");

        // Assert
        address1.Should().Be(address2);
    }
}

