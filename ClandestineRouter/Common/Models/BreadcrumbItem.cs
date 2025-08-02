namespace ClandestineRouter.Common.Models;

public class BreadcrumbItem
{
    public string Text { get; set; } = string.Empty;
    public string? Href { get; set; }
    public bool IsActive { get; set; }
    public string? Icon { get; set; }
}