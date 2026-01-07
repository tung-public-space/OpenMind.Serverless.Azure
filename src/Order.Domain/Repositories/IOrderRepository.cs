namespace Order.Domain.Repositories;

/// <summary>
/// Repository interface for Order aggregate
/// </summary>
public interface IOrderRepository
{
    Task<Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Order>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

