namespace ClandestineRouter.Services.DataService;
#region Supporting Classes and Enums

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

#endregion