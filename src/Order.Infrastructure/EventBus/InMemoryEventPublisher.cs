using Microsoft.Extensions.Logging;
using Order.Domain.Events;

namespace Order.Infrastructure.EventBus;

/// <summary>
/// In-memory event publisher for development and testing.
/// Replace with actual message broker (e.g., Azure Service Bus, EventGrid) in production.
/// </summary>
public class InMemoryEventPublisher : IEventPublisher
{
    private readonly ILogger<InMemoryEventPublisher> _logger;

    public InMemoryEventPublisher(ILogger<InMemoryEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        _logger.LogInformation("Publishing event: {EventType} - {EventId}", 
            typeof(T).Name, 
            domainEvent.EventId);
        
        // In a real implementation, this would publish to Azure Service Bus, EventGrid, etc.
        return Task.CompletedTask;
    }

    public async Task PublishManyAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await PublishAsync(domainEvent, cancellationToken);
        }
    }
}

