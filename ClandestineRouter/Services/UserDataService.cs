using ClandestineRouter.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ClandestineRouter.Services;
public class UserDataService(
    IHttpContextAccessor httpContextAccessor,
    AuthenticationStateProvider authStateProvider,
    UserManager<ApplicationUser> userManager)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly AuthenticationStateProvider _authStateProvider = authStateProvider;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private ApplicationUser? _cachedUser;

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        if (_cachedUser != null)
            return _cachedUser;

        ClaimsPrincipal? principal = null;

        // Try HttpContext first (available during prerendering)
        if (_httpContextAccessor.HttpContext != null)
        {
            principal = _httpContextAccessor.HttpContext.User;
        }
        else
        {
            // Fallback to AuthenticationStateProvider (during interactive rendering)
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            principal = authState.User;
        }

        if (principal?.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _cachedUser = await _userManager.FindByIdAsync(userId);
            }
        }

        return _cachedUser;
    }

    public void UpdateCache(ApplicationUser user)
    {
        _cachedUser = user;
    }

    public void ClearCache()
    {
        _cachedUser = null;
    }
}