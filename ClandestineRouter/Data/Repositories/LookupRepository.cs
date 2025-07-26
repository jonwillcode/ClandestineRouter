using ClandestineRouter.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ClandestineRouter.Data.Repositories;

public interface ILookupRepository<T> where T : class, IBaseLookupModel
{
    Task<List<T>> GetAllActiveAsync();
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}

public class LookupRepository<T>(ApplicationDbContext context) : ILookupRepository<T> where T : class, IBaseLookupModel
{
    private readonly ApplicationDbContext _dbContext = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<List<T>> GetAllActiveAsync()
    {
        return await _dbSet.Where(x => x.IsActive)
                          .OrderBy(x => x.Name)
                          .ToListAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
