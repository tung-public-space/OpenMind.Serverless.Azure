namespace Order.Domain.Events;

public record OrderCreatedEvent(Guid OrderId, string CustomerId) : DomainEventBase;

public record OrderItemAddedEvent(Guid OrderId, string ProductId, int Quantity) : DomainEventBase;

public record OrderItemRemovedEvent(Guid OrderId, string ProductId) : DomainEventBase;

public record OrderConfirmedEvent(Guid OrderId) : DomainEventBase;

public record OrderShippedEvent(Guid OrderId) : DomainEventBase;

public record OrderDeliveredEvent(Guid OrderId) : DomainEventBase;

public record OrderCancelledEvent(Guid OrderId, string Reason) : DomainEventBase;

