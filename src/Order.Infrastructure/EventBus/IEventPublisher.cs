using Order.Domain.Events;

namespace Order.Infrastructure.EventBus;

/// <summary>
/// Interface for publishing domain events
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent;
    Task PublishManyAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

