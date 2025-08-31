using ClandestineRouter.Data;
using ClandestineRouter.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using DataAnnotationValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace ClandestineRouter.Services;

/// <summary>
/// Data service with security, caching, and comprehensive error handling.
/// </summary>
public interface IDataService<TEntity> where TEntity : class, IBaseModel
{
    Task<ServiceResult<TEntity?>> GetByIdAsync(Guid id, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<IEnumerable<TEntity>>> GetAllAsync(ClaimsPrincipal? user = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<PagedResult<TEntity>>> GetPagedAsync(int page, int pageSize, Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null, bool ascending = true, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<TEntity>> CreateAsync(TEntity entity, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<TEntity>> UpdateAsync(TEntity entity, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<IEnumerable<TEntity>>> SearchAsync(Expression<Func<TEntity, bool>> predicate, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default);
}

public class DataService<TEntity> : IDataService<TEntity> where TEntity : class, IBaseModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataService<TEntity>> _logger;
    private readonly IMemoryCache _cache;
    private readonly DataServiceOptions _options;
    private readonly DbSet<TEntity> _dbSet;
    private readonly string _entityName;

    public DataService(
        ApplicationDbContext context,
        ILogger<DataService<TEntity>> logger,
        IMemoryCache cache,
        IOptions<DataServiceOptions> options)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _dbSet = _context.Set<TEntity>();
        _entityName = typeof(TEntity).Name;
    }

    public async Task<ServiceResult<TEntity?>> GetByIdAsync(Guid id, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid ID provided: {Id} for entity {EntityName}", id, _entityName);
                return ServiceResult<TEntity?>.Failure("Invalid ID provided", ServiceErrorType.ValidationError);
            }

            var cacheKey = $"{_entityName}_{id}";

            if (_cache.TryGetValue(cacheKey, out TEntity? cachedEntity))
            {
                _logger.LogDebug("Cache hit for {EntityName} with ID {Id}", _entityName, id);

                if (cachedEntity != null && !await CanAccessEntityAsync(cachedEntity, user, OperationType.Read))
                {
                    _logger.LogWarning("Access denied for user {UserId} to {EntityName} {Id}",
                        GetUserId(user), _entityName, id);
                    return ServiceResult<TEntity?>.Failure("Access denied", ServiceErrorType.UnauthorizedAccess);
                }

                return ServiceResult<TEntity?>.Success(cachedEntity);
            }

            var query = ApplySoftDeleteFilter(_dbSet.AsQueryable());
            var entity = await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity != null && !await CanAccessEntityAsync(entity, user, OperationType.Read))
            {
                _logger.LogWarning("Access denied for user {UserId} to {EntityName} {Id}",
                    GetUserId(user), _entityName, id);
                return ServiceResult<TEntity?>.Failure("Access denied", ServiceErrorType.UnauthorizedAccess);
            }

            if (entity != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheExpirationMinutes),
                    SlidingExpiration = TimeSpan.FromMinutes(_options.CacheSlidingExpirationMinutes),
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, entity, cacheOptions);
                _logger.LogDebug("Cached {EntityName} with ID {Id}", _entityName, id);
            }

            _logger.LogInformation("Retrieved {EntityName} with ID {Id} by user {UserId}",
                _entityName, id, GetUserId(user));

            return ServiceResult<TEntity?>.Success(entity);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("GetByIdAsync operation was cancelled for {EntityName} {Id}", _entityName, id);
            return ServiceResult<TEntity?>.Failure("Operation was cancelled", ServiceErrorType.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {EntityName} with ID {Id}", _entityName, id);
            return ServiceResult<TEntity?>.Failure($"Error retrieving {_entityName}", ServiceErrorType.DatabaseError);
        }
    }

    public async Task<ServiceResult<IEnumerable<TEntity>>> GetAllAsync(ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await CanPerformOperationAsync(user, OperationType.ReadAll))
            {
                _logger.LogWarning("Access denied for user {UserId} to read all {EntityName}",
                    GetUserId(user), _entityName);
                return ServiceResult<IEnumerable<TEntity>>.Failure("Access denied", ServiceErrorType.UnauthorizedAccess);
            }

            var cacheKey = $"{_entityName}_All_{GetUserId(user)}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<TEntity>? cachedEntities))
            {
                _logger.LogDebug("Cache hit for all {EntityName} entities", _entityName);
                return ServiceResult<IEnumerable<TEntity>>.Success(cachedEntities!);
            }

            var query = ApplySoftDeleteFilter(_dbSet.AsQueryable());
            query = await ApplyUserFiltersAsync(query, user, OperationType.ReadAll);

            var entities = await query.ToListAsync(cancellationToken);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheExpirationMinutes),
                Priority = CacheItemPriority.Low
            };

            _cache.Set(cacheKey, entities, cacheOptions);

            _logger.LogInformation("Retrieved all {EntityName} entities ({Count} items) for user {UserId}",
                _entityName, entities.Count, GetUserId(user));

            return ServiceResult<IEnumerable<TEntity>>.Success(entities);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("GetAllAsync operation was cancelled for {EntityName}", _entityName);
            return ServiceResult<IEnumerable<TEntity>>.Failure("Operation was cancelled", ServiceErrorType.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all {EntityName} entities", _entityName);
            return ServiceResult<IEnumerable<TEntity>>.Failure($"Error retrieving {_entityName} entities", ServiceErrorType.DatabaseError);
        }
    }

    public async Task<ServiceResult<PagedResult<TEntity>>> GetPagedAsync(int page, int pageSize,
        Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = _options.DefaultPageSize;
            if (pageSize > _options.MaxPageSize) pageSize = _options.MaxPageSize;

            if (!await CanPerformOperationAsync(user, OperationType.ReadAll))
            {
                _logger.LogWarning("Access denied for user {UserId} to read paged {EntityName}",
                    GetUserId(user), _entityName);
                return ServiceResult<PagedResult<TEntity>>.Failure("Access denied", ServiceErrorType.UnauthorizedAccess);
            }

            var query = ApplySoftDeleteFilter(_dbSet.AsQueryable());
            query = await ApplyUserFiltersAsync(query, user, OperationType.ReadAll);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            var entities = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<TEntity>
            {
                Items = entities,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            _logger.LogInformation("Retrieved paged {EntityName} entities (Page {Page}/{TotalPages}, {Count}/{TotalCount} items) for user {UserId}",
                _entityName, page, pagedResult.TotalPages, entities.Count, totalCount, GetUserId(user));

            return ServiceResult<PagedResult<TEntity>>.Success(pagedResult);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("GetPagedAsync operation was cancelled for {EntityName}", _entityName);
            return ServiceResult<PagedResult<TEntity>>.Failure("Operation was cancelled", ServiceErrorType.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged {EntityName} entities", _entityName);
            return ServiceResult<PagedResult<TEntity>>.Failure($"Error retrieving paged {_entityName} entities", ServiceErrorType.DatabaseError);
        }
    }

    public async Task<ServiceResult<TEntity>> CreateAsync(TEntity entity, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            return ServiceResult<TEntity>.Failure("Entity cannot be null", ServiceErrorType.ValidationError);
        }

        try
        {
            if (!await CanPerformOperationAsync(user, OperationType.Create))
            {
                _logger.LogWarning("Access denied for user {UserId} to create {EntityName}",
                    GetUserId(user), _entityName);
                return ServiceResult<TEntity>.Failure("Access denied", ServiceErrorType.UnauthorizedAccess);
            }

            var validationResult = ValidateEntity(entity);
            if (!validationResult.IsSuccess)
            {
                return ServiceResult<TEntity>.Failure(validationResult.ErrorMessage!, ServiceErrorType.ValidationError);
            }

            // Set audit fields
            SetAuditFields(entity, user, isCreate: true);

            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Invalidate relevant caches
            InvalidateEntityCaches();

            var entityId = GetEntityId(entity);
            _logger.LogInformation("Created {EntityName} with ID {Id} by user {UserId}",
                _entityName, entityId, GetUserId(user));

            return ServiceResult<TEntity>.Success(entity);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error creating {EntityName}", _entityName);
            return ServiceResult<TEntity>.Failure($"Database error creating {_entityName}", ServiceErrorType.DatabaseError);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("CreateAsync operation was cancelled for {EntityName}", _entityName);
            return ServiceResult<TEntity>.Failure("Operation was cancelled", ServiceErrorType.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {EntityName}", _entityName);
            return ServiceResult<TEntity>.Failure($"Error creating {_entityName}", ServiceErrorType.UnknownError);
        }
    }

    public async Task<ServiceResult<TEntity>> UpdateAsync(TEntity entity, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            return ServiceResult<TEntity>.Failure("Entity cannot be null", ServiceErrorType.ValidationError);
        }

        try
        {
            var entityId = GetEntityId(entity);

            if (!await CanAccessEntityAsync(entity, user, OperationType.Update))
            {
                _logger.LogWarning("Access denied for user {UserId} to update {EntityName} {Id}",
                    GetUserId(user), _entityName, entityId);
                return ServiceResult<TEntity>.Failure("Access denied", ServiceErrorType.UnauthorizedAccess);
            }

            var validationResult = ValidateEntity(entity);
            if (!validationResult.IsSuccess)
            {
                return ServiceResult<TEntity>.Failure(validationResult.ErrorMessage!, ServiceErrorType.ValidationError);
            }

            // Set audit fields
            SetAuditFields(entity, user, isCreate: false);

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);

            // Invalidate relevant caches
            InvalidateEntityCaches();
            _cache.Remove($"{_entityName}_{entityId}");

            _logger.LogInformation("Updated {EntityName} with ID {Id} by user {UserId}",
                _entityName, entityId, GetUserId(user));

            return ServiceResult<TEntity>.Success(entity);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict updating {EntityName}", _entityName);
            return ServiceResult<TEntity>.Failure("The entity was modified by another user. Please refresh and try again.", ServiceErrorType.ConcurrencyError);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error updating {EntityName}", _entityName);
            return ServiceResult<TEntity>.Failure($"Database error updating {_entityName}", ServiceErrorType.DatabaseError);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("UpdateAsync operation was cancelled for {EntityName}", _entityName);
            return ServiceResult<TEntity>.Failure("Operation was cancelled", ServiceErrorType.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {EntityName}", _entityName);
            return ServiceResult<TEntity>.Failure($"Error updating {_entityName}", ServiceErrorType.UnknownError);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return ServiceResult<bool>.Failure("Invalid ID provided", ServiceErrorType.ValidationError);
            }

            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure($"{_entityName} not found", ServiceErrorType.NotFound);
            }

            if (!await CanAccessEntityAsync(entity, user, OperationType.Delete))
            {
                _logger.LogWarning("Access denied for user {UserId} to delete {EntityName} {Id}",
                    GetUserId(user), _entityName, id);
                return ServiceResult<bool>.Failure("Access denied", ServiceErrorType.UnauthorizedAccess);
            }

            if (entity is ISoftDeletableEntity softDeletableEntity && _options.UseSoftDelete)
            {
                // Soft delete
                softDeletableEntity.IsActive = false;
                SetAuditFields(entity, user, isCreate: false);
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                // Hard delete
                _dbSet.Remove(entity);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Invalidate relevant caches
            InvalidateEntityCaches();
            _cache.Remove($"{_entityName}_{id}");

            _logger.LogInformation("Deleted {EntityName} with ID {Id} by user {UserId} (Soft: {SoftDelete})",
                _entityName, id, GetUserId(user), _options.UseSoftDelete);

            return ServiceResult<bool>.Success(true);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error deleting {EntityName} {Id}", _entityName, id);
            return ServiceResult<bool>.Failure($"Database error deleting {_entityName}", ServiceErrorType.DatabaseError);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("DeleteAsync operation was cancelled for {EntityName} {Id}", _entityName, id);
            return ServiceResult<bool>.Failure("Operation was cancelled", ServiceErrorType.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {EntityName} {Id}", _entityName, id);
            return ServiceResult<bool>.Failure($"Error deleting {_entityName}", ServiceErrorType.UnknownError);
        }
    }

    public async Task<ServiceResult<IEnumerable<TEntity>>> SearchAsync(Expression<Func<TEntity, bool>> predicate,
        ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return ServiceResult<IEnumerable<TEntity>>.Failure("Search predicate cannot be null", ServiceErrorType.ValidationError);
        }

        try
        {
            if (!await CanPerformOperationAsync(user, OperationType.ReadAll))
            {
                _logger.LogWarning("Access denied for user {UserId} to search {EntityName}",
                    GetUserId(user), _entityName);
                return ServiceResult<IEnumerable<TEntity>>.Failure("Access denied", ServiceErrorType.UnauthorizedAccess);
            }

            var query = ApplySoftDeleteFilter(_dbSet.AsQueryable());
            query = await ApplyUserFiltersAsync(query, user, OperationType.ReadAll);
            query = query.Where(predicate);

            var entities = await query.Take(_options.MaxSearchResults).ToListAsync(cancellationToken);

            _logger.LogInformation("Searched {EntityName} entities, found {Count} results for user {UserId}",
                _entityName, entities.Count, GetUserId(user));

            return ServiceResult<IEnumerable<TEntity>>.Success(entities);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("SearchAsync operation was cancelled for {EntityName}", _entityName);
            return ServiceResult<IEnumerable<TEntity>>.Failure("Operation was cancelled", ServiceErrorType.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching {EntityName} entities", _entityName);
            return ServiceResult<IEnumerable<TEntity>>.Failure($"Error searching {_entityName} entities", ServiceErrorType.DatabaseError);
        }
    }

    #region Private Helper Methods

    private IQueryable<TEntity> ApplySoftDeleteFilter(IQueryable<TEntity> query)
    {
        if (typeof(ISoftDeletableEntity).IsAssignableFrom(typeof(TEntity)) && _options.UseSoftDelete)
        {
            return query.Where(e => ((ISoftDeletableEntity)e).IsActive);
        }
        return query;
    }

    private async Task<IQueryable<TEntity>> ApplyUserFiltersAsync(IQueryable<TEntity> query, ClaimsPrincipal? user, OperationType operation)
    {
        // Apply tenant isolation if entity supports audit tracking
        if (typeof(IAuditableEntity).IsAssignableFrom(typeof(TEntity)) && _options.EnableTenantIsolation && user != null)
        {
            var userId = GetUserId(user);
            if (userId.HasValue && !IsAdmin(user))
            {
                query = query.Where(e => ((IAuditableEntity)e).CreatedById == userId.Value);
            }
        }

        return await Task.FromResult(query);
    }

    private async Task<bool> CanPerformOperationAsync(ClaimsPrincipal? user, OperationType operation)
    {
        if (!_options.EnableAuthorization) return true;
        if (user == null) return false;

        // Admin users can perform all operations
        if (IsAdmin(user)) return true;

        // Check specific permissions based on operation
        return operation switch
        {
            OperationType.Read => user.HasClaim("permission", $"{_entityName}:read") || user.HasClaim("permission", "all:read"),
            OperationType.ReadAll => user.HasClaim("permission", $"{_entityName}:read-all") || user.HasClaim("permission", "all:read-all"),
            OperationType.Create => user.HasClaim("permission", $"{_entityName}:create") || user.HasClaim("permission", "all:create"),
            OperationType.Update => user.HasClaim("permission", $"{_entityName}:update") || user.HasClaim("permission", "all:update"),
            OperationType.Delete => user.HasClaim("permission", $"{_entityName}:delete") || user.HasClaim("permission", "all:delete"),
            _ => false
        };
    }

    private async Task<bool> CanAccessEntityAsync(TEntity entity, ClaimsPrincipal? user, OperationType operation)
    {
        if (!await CanPerformOperationAsync(user, operation)) return false;

        // If tenant isolation is enabled and user is not admin, check ownership
        if (entity is IAuditableEntity auditableEntity && _options.EnableTenantIsolation && user != null && !IsAdmin(user))
        {
            var entityCreatedBy = auditableEntity.CreatedById;
            var userId = GetUserId(user);
            return userId.HasValue && entityCreatedBy == userId.Value;
        }

        return true;
    }

    private bool IsAdmin(ClaimsPrincipal user)
    {
        return user.IsInRole("Admin") || user.HasClaim("role", "admin");
    }

    private Guid? GetUserId(ClaimsPrincipal? user)
    {
        if (user == null) return null;

        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdString))
            return null;

        return Guid.TryParse(userIdString, out var userId) ? userId : null;
    }

    private Guid GetEntityId(TEntity entity)
    {
        return entity.Id;
    }

    private void SetAuditFields(TEntity entity, ClaimsPrincipal? user, bool isCreate)
    {
        var userId = GetUserId(user);
        var now = DateTime.UtcNow;

        // Always update the UpdatedDateTimeUtc (from your BaseModel)
        entity.UpdatedDateTimeUtc = now;

        // Handle audit fields if entity supports them
        if (entity is IAuditableEntity auditableEntity)
        {
            if (isCreate)
            {
                auditableEntity.CreatedById = userId;
                // CreatedDateTimeUtc is already set in BaseModel constructor
            }
            else
            {
                auditableEntity.ModifiedById = userId;
            }
        }
    }

    private ServiceValidationResult ValidateEntity(TEntity entity)
    {
        var validationContext = new ValidationContext(entity);
        var validationResults = new List<DataAnnotationValidationResult>();

        if (!Validator.TryValidateObject(entity, validationContext, validationResults, true))
        {
            var errors = string.Join("; ", validationResults.Select(vr => vr.ErrorMessage));
            return new ServiceValidationResult { IsSuccess = false, ErrorMessage = errors };
        }

        return new ServiceValidationResult { IsSuccess = true };
    }

    private void InvalidateEntityCaches()
    {
        // In a real implementation, you might want to use a more sophisticated cache invalidation strategy
        // For now, we'll remove common cache patterns
        var keysToRemove = new List<string>
        {
            $"{_entityName}_All",
            $"{_entityName}_Paged"
        };

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
        }
    }

    #endregion
}

#region Supporting Classes and Enums

public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public ServiceErrorType ErrorType { get; private set; }

    private ServiceResult(bool isSuccess, T? data, string? errorMessage, ServiceErrorType errorType)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static ServiceResult<T> Success(T data) => new(true, data, null, ServiceErrorType.None);
    public static ServiceResult<T> Failure(string errorMessage, ServiceErrorType errorType) => new(false, default, errorMessage, errorType);
}

public enum ServiceErrorType
{
    None,
    ValidationError,
    UnauthorizedAccess,
    NotFound,
    DatabaseError,
    ConcurrencyError,
    OperationCancelled,
    UnknownError
}

public enum OperationType
{
    Read,
    ReadAll,
    Create,
    Update,
    Delete
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}

public class ServiceValidationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}

public class DataServiceOptions
{
    public bool EnableAuthorization { get; set; } = true;
    public bool EnableTenantIsolation { get; set; } = true;
    public bool UseSoftDelete { get; set; } = true;
    public int CacheExpirationMinutes { get; set; } = 30;
    public int CacheSlidingExpirationMinutes { get; set; } = 5;
    public int DefaultPageSize { get; set; } = 20;
    public int MaxPageSize { get; set; } = 100;
    public int MaxSearchResults { get; set; } = 1000;
}

#endregion