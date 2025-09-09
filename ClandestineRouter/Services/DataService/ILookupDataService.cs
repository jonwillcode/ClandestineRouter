using ClandestineRouter.Data.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ClandestineRouter.Services.DataService;

public interface ILookupDataService<TEntity> where TEntity : class, ILookupEntity
{
    Task<ServiceResult<TEntity?>> GetByIdAsync(Guid id,
        ClaimsPrincipal? user = null,
        CancellationToken cancellationToken = default
        );
    Task<ServiceResult<IEnumerable<TEntity>>> GetAllAsync(ClaimsPrincipal? user = null,
        CancellationToken cancellationToken = default
        );
    Task<ServiceResult<PagedResult<TEntity>>> GetPagedAsync(int page,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        ClaimsPrincipal? user = null,
        CancellationToken cancellationToken = default
        );
    Task<ServiceResult<TEntity>> CreateAsync(TEntity entity,
        ClaimsPrincipal? user = null,
        CancellationToken cancellationToken = default
        );
    Task<ServiceResult<TEntity>> UpdateAsync(TEntity entity,
        ClaimsPrincipal? user = null,
        CancellationToken cancellationToken = default
        );
    Task<ServiceResult<bool>> DeleteAsync(Guid id,
        ClaimsPrincipal? user = null,
        CancellationToken cancellationToken = default
        );
    Task<ServiceResult<IEnumerable<TEntity>>> SearchAsync(Expression<Func<TEntity, bool>> predicate,
        ClaimsPrincipal? user = null,
        CancellationToken cancellationToken = default
        );
}
