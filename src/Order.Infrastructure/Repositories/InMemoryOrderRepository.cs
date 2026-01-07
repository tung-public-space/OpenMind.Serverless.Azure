using System.Collections.Concurrent;
using Order.Domain.Repositories;

namespace Order.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of IOrderRepository for development and testing.
/// Replace with actual database implementation (e.g., CosmosDB, SQL Server) in production.
/// </summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Domain.Entities.Order> _orders = new();

    public Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<IEnumerable<Domain.Entities.Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        var orders = _orders.Values.Where(o => o.CustomerId == customerId);
        return Task.FromResult(orders);
    }

    public Task<IEnumerable<Domain.Entities.Order>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        var orders = _orders.Values.Skip(skip).Take(take);
        return Task.FromResult(orders);
    }

    public Task AddAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default)
    {
        _orders.TryAdd(order.Id, order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _orders.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}

