using Order.Application.DTOs;
using Order.Domain.Entities;
using Order.Domain.ValueObjects;

namespace Order.Application.Mappers;

/// <summary>
/// Maps domain entities to DTOs
/// </summary>
public static class OrderMapper
{
    public static OrderDto ToDto(this Domain.Entities.Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            ShippingAddress = order.ShippingAddress.ToDto(),
            BillingAddress = order.BillingAddress?.ToDto(),
            TotalAmount = order.TotalAmount.Amount,
            Currency = order.TotalAmount.Currency,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => i.ToDto()).ToList()
        };
    }

    public static OrderItemDto ToDto(this OrderItem item)
    {
        return new OrderItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice.Amount,
            TotalPrice = item.TotalPrice.Amount,
            Currency = item.UnitPrice.Currency
        };
    }

    public static AddressDto ToDto(this Address address)
    {
        return new AddressDto
        {
            Street = address.Street,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    public static Address ToDomain(this AddressDto dto)
    {
        return new Address(dto.Street, dto.City, dto.State, dto.PostalCode, dto.Country);
    }
}

