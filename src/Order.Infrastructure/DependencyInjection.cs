using Microsoft.Extensions.DependencyInjection;
using Order.Domain.Repositories;
using Order.Infrastructure.EventBus;
using Order.Infrastructure.Repositories;

namespace Order.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register repositories (use singleton for in-memory, scoped for real databases)
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        // Register event publisher
        services.AddSingleton<IEventPublisher, InMemoryEventPublisher>();

        return services;
    }
}

