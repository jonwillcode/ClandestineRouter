using ClandestineRouter.Data.Models;
using ClandestineRouter.Services;
using System.Reflection;

namespace ClandestineRouter.Common.Extensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        // If no assemblies provided, scan the calling assembly 
        if (!assemblies.Any())
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        // Find all entity types that implement IEntity 
        var entityTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                typeof(IEntity).IsAssignableFrom(t))
            .ToList();

        // Register IDataService<T> for each entity type 
        foreach (var entityType in entityTypes)
        {
            var serviceType = typeof(IDataService<>).MakeGenericType(entityType);
            var implementationType = typeof(DataService<>).MakeGenericType(entityType);

            services.AddScoped(serviceType, implementationType);
        }

        var entityNames = string.Join(", ", entityTypes.Select(t => t.Name));
        Console.WriteLine($"Auto-Registered IDataService for {entityTypes.Count} entities: {entityNames}");

        return services; 
    }

    public static IServiceCollection AddDataServiceStack(this IServiceCollection services, Action<DataServiceOptions>? configureOptions = null, params Assembly[] assemblies)
    {
        // Add memory cache (required by DataService)
        services.AddMemoryCache();

        // Configure options
        services.Configure<DataServiceOptions>(options =>
        {
            // Set sensible defaults
            options.EnableAuthorization = false;
            options.EnableTenantIsolation = false;
            options.UseSoftDelete = false;
            options.CacheExpirationMinutes = 30;
            options.CacheSlidingExpirationMinutes = 5;
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.MaxSearchResults = 1000;

            // Apply custom configuration
            configureOptions?.Invoke(options);
        });

        // Register all data services
        services.AddDataServices(assemblies);

        return services;
    }
}
