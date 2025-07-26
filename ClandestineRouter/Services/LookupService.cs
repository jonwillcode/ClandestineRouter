using ClandestineRouter.Data.Models;
using ClandestineRouter.Data.Repositories;

namespace ClandestineRouter.Services;

public interface ILookupService
{
    Task<List<T>> GetLookupItemsAsync<T>() where T : class, IBaseLookupModel;
    Task<T?> GetLookupItemAsync<T>(Guid id) where T : class, IBaseLookupModel;
}

public class LookupService(IServiceProvider serviceProvider) : ILookupService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<List<T>> GetLookupItemsAsync<T>() where T : class, IBaseLookupModel
    {
        var repository = _serviceProvider.GetRequiredService<ILookupRepository<T>>();
        return await repository.GetAllActiveAsync();
    }

    public async Task<T?> GetLookupItemAsync<T>(Guid id) where T : class, IBaseLookupModel
    {
        var repository = _serviceProvider.GetRequiredService<ILookupRepository<T>>();
        return await repository.GetByIdAsync(id);
    }
}
