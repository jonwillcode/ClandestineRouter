using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClandestineRouter.Services.DataService;

/// <summary>
/// Helper methods for creating include functions for the data service
/// </summary>
public static class IncludeHelpers
{
    /// <summary>
    /// Creates an include function for a single navigation property
    /// </summary>
    public static Func<IQueryable<TEntity>, IQueryable<TEntity>> Include<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> navigationProperty)
        where TEntity : class
    {
        return query => query.Include(navigationProperty);
    }

    /// <summary>
    /// Creates an include function for multiple navigation properties
    /// </summary>
    public static Func<IQueryable<TEntity>, IQueryable<TEntity>> IncludeMultiple<TEntity>(
        params Expression<Func<TEntity, object>>[] navigationProperties)
        where TEntity : class
    {
        return query =>
        {
            var result = query;
            foreach (var navigationProperty in navigationProperties)
            {
                result = result.Include(navigationProperty);
            }
            return result;
        };
    }

    /// <summary>
    /// Creates an include function using string property names (useful for dynamic scenarios)
    /// </summary>
    public static Func<IQueryable<TEntity>, IQueryable<TEntity>> IncludeByNames<TEntity>(
        params string[] propertyNames)
        where TEntity : class
    {
        return query =>
        {
            var result = query;
            foreach (var propertyName in propertyNames)
            {
                result = result.Include(propertyName);
            }
            return result;
        };
    }

    /// <summary>
    /// Creates an include function with ThenInclude for nested properties
    /// </summary>
    public static Func<IQueryable<TEntity>, IQueryable<TEntity>> IncludeThen<TEntity, TProperty, TNested>(
        Expression<Func<TEntity, TProperty>> navigationProperty,
        Expression<Func<TProperty, TNested>> thenIncludeProperty)
        where TEntity : class
    {
        return query => query.Include(navigationProperty).ThenInclude(thenIncludeProperty);
    }

    /// <summary>
    /// Combines multiple include functions
    /// </summary>
    public static Func<IQueryable<TEntity>, IQueryable<TEntity>> Combine<TEntity>(
        params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includeFunctions)
        where TEntity : class
    {
        return query =>
        {
            var result = query;
            foreach (var includeFunction in includeFunctions)
            {
                result = includeFunction(result);
            }
            return result;
        };
    }

    /// <summary>
    /// Creates a custom include function using a lambda expression
    /// </summary>
    public static Func<IQueryable<TEntity>, IQueryable<TEntity>> Custom<TEntity>(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> customIncludeFunction)
        where TEntity : class
    {
        return customIncludeFunction;
    }
}
