using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class SocialMediaAccount : CommonEntityBase
{
    public SocialMediaApp SocialMediaApp { get; set; } = null!;
    [Required]
    public Guid SocialMediaAppId { get; set; }

    [Required, MaxLength(256)]
    public required string Username { get; set; }

    [MaxLength(256)]
    public string? DisplayName { get; set; }

    [MaxLength(2000)]
    public string? Bio {  get; set; }

    public string? Notes { get; set; }
}
