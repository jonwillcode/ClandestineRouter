namespace ClandestineRouter.Services.DataService;
#region Supporting Classes and Enums

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