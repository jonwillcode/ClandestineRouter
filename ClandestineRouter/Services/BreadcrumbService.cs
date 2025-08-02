using ClandestineRouter.Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace ClandestineRouter.Services;

public class BreadcrumbService : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly List<BreadcrumbItem> _breadcrumbs = new();

    public event Action? OnBreadcrumbsChanged;

    public IReadOnlyList<BreadcrumbItem> Breadcrumbs => _breadcrumbs.AsReadOnly();

    public BreadcrumbService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public void AddBreadcrumb(BreadcrumbItem breadcrumb)
    {
        _breadcrumbs.Add(breadcrumb);
        OnBreadcrumbsChanged?.Invoke();
    }

    public void ClearBreadcrumbs()
    {
        _breadcrumbs.Clear();
        OnBreadcrumbsChanged?.Invoke();
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }

    public void SetBreadcrumbs(params BreadcrumbItem[] breadcrumbs)
    {
        _breadcrumbs.Clear();
        _breadcrumbs.AddRange(breadcrumbs);
        OnBreadcrumbsChanged?.Invoke();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        GenerateBreadcrumbs(e.Location);
        OnBreadcrumbsChanged?.Invoke();
    }

    private void GenerateBreadcrumbs(string url)
    {
        _breadcrumbs.Clear();

        var uri = new Uri(url);
        var segments = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Always add home
        _breadcrumbs.Add(new BreadcrumbItem
        {
            Text = "Home",
            Href = "/",
            Icon = "🏠"
        });

        var currentPath = "";
        for (int i = 0; i < segments.Length; i++)
        {
            currentPath += "/" + segments[i];
            var isLast = i == segments.Length - 1;

            _breadcrumbs.Add(new BreadcrumbItem
            {
                Text = FormatSegmentText(segments[i]),
                Href = isLast ? null : currentPath,
                IsActive = isLast
            });
        }
    }

    private static string FormatSegmentText(string segment)
    {
        // Convert URL segment to readable text
        return segment.Replace("-", " ")
                     .Replace("_", " ")
                     .Split(' ')
                     .Select(word => char.ToUpper(word[0]) + word[1..].ToLower())
                     .Aggregate((a, b) => a + " " + b);
    }
}
