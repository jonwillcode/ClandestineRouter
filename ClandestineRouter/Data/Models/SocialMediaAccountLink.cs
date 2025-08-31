using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class SocialMediaAccountLink : CommonEntityBase
{
    [Required]
    public required Guid AccountId { get; set; }

    public SocialMediaAccount SocialMediaAccount { get; set; } = null!;

    [Required, MaxLength(256)]
    public required string Link { get; set; }
}
