using ClandestineRouter.Data;
using ClandestineRouter.Data.Models;
using ClandestineRouter.Services.DataService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ClandestineRouter.Services;

public class SocialMediaAccountDataService(
    ApplicationDbContext dbContext, 
    ILogger<SocialMediaAccountDataService> logger, 
    IOptions<DataServiceOptions> options
    ) : IDataService<SocialMediaAccount>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<SocialMediaAccountDataService> _logger = logger;
    private readonly DataServiceOptions _options = options.Value;
    private readonly DbSet<SocialMediaAccount> _dbSet = dbContext.Set<SocialMediaAccount>();

    public Task<ServiceResult<SocialMediaAccount>> CreateAsync(SocialMediaAccount entity, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<bool>> DeleteAsync(Guid id, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<IEnumerable<SocialMediaAccount>>> GetAllAsync(ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO add CanPerformOperationAsync check here

            // TODO add a cache key 

            // TODO Lookup from cache if exists 

            // TODO build query using ApplySoftDeletedFilter
            var query = _dbSet.Include(x => x.SocialMediaApp)
                .AsQueryable()
                .AsNoTracking();

            // TODO apply user filter? User ID should be supplied when querying, right?

            // TODO create the entities variable and set it to the result of calling query.ToListAsync(cancellationToken)
            var entities = await query.ToListAsync(cancellationToken);

            // TODO Set cache options and store in cache 

            // Log information about the retrieval
            _logger.LogInformation("Retreived all {EntityName} entities ({Count} items) for user {UserId}", nameof(SocialMediaAccount), entities.Count, user?.Identity?.Name ?? "Unknown");

            // Return the ServiceResult
            return ServiceResult<IEnumerable<SocialMediaAccount>>.Success(entities);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("GetAllAsync operation was cancelled for SocialMediaAccount");
            return ServiceResult<IEnumerable<SocialMediaAccount>>.Failure("Operation was cancelled", ServiceErrorType.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all SocialMediaAccounts");
            return ServiceResult<IEnumerable<SocialMediaAccount>>.Failure($"Error retrieving SociaMediaAccount entities", ServiceErrorType.DatabaseError);
        }
    }

    public Task<ServiceResult<SocialMediaAccount?>> GetByIdAsync(Guid id, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<PagedResult<SocialMediaAccount>>> GetPagedAsync(int page, int pageSize, Expression<Func<SocialMediaAccount, bool>>? filter = null, Expression<Func<SocialMediaAccount, object>>? orderBy = null, bool ascending = true, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<IEnumerable<SocialMediaAccount>>> SearchAsync(Expression<Func<SocialMediaAccount, bool>> predicate, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<SocialMediaAccount>> UpdateAsync(SocialMediaAccount entity, ClaimsPrincipal? user = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
