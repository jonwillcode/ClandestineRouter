using ClandestineRouter.Data.Models;
using ClandestineRouter.Services.DataService;
using System.Reflection;

namespace ClandestineRouter.Common.Extensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        // If no assemblies provided, scan the calling assembly 
        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        // Find all entity types that implement ILookupEntity 
        var lookupEntityTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                typeof(ILookupEntity).IsAssignableFrom(t))
            .ToList();

        // Register IDataService<T> for each lookup entity type 
        foreach (var entityType in lookupEntityTypes)
        {
            var serviceType = typeof(IDataService<>).MakeGenericType(entityType);
            var implementationType = typeof(LookupDataService<>).MakeGenericType(entityType);
            services.AddScoped(serviceType, implementationType);
        }

        var lookupEntityNames = string.Join(", ", lookupEntityTypes.Select(t => t.Name));
        Console.WriteLine($"Auto-Registered IDataService for {lookupEntityTypes.Count} ILookupEntity entities: {lookupEntityNames}");

        // Find all entity types that implement ICommonEntity 
        var commonEntityTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericType &&
                typeof(ICommonEntity).IsAssignableFrom(t))
            .ToList();

        // Register ICommonDataService<T> for each common entity type 
        foreach (var entityType in commonEntityTypes)
        {
            var commonServiceType = typeof(ICommonDataService<>).MakeGenericType(entityType);
            var dataServiceType = typeof(IDataService<>).MakeGenericType(entityType);
            var implementationType = typeof(CommonDataService<>).MakeGenericType(entityType);

            // Register both interfaces to the same implementation
            services.AddScoped(commonServiceType, implementationType);
            services.AddScoped(dataServiceType, implementationType);
        }

        var commonEntityNames = string.Join(", ", commonEntityTypes.Select(t => t.Name));
        Console.WriteLine($"Auto-Registered IDataService for {commonEntityTypes.Count} ICommonEntity entities: {commonEntityNames}");

        return services;
    }
    public static IServiceCollection AddDataServiceStack(this IServiceCollection services, Action<DataServiceOptions>? configureOptions = null, params Assembly[] assemblies)
    {
        // Add memory cache (required by LookupDataService)
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
